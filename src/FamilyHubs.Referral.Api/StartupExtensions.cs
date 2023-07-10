using AutoMapper;
using AutoMapper.EquivalencyExpression;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Api.Endpoints;
using FamilyHubs.Referral.Api.Middleware;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.Referral.Data.Repository;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using FamilyHubs.SharedKernel.GovLogin.AppStart;
using FamilyHubs.SharedKernel.Identity;
using System.Diagnostics.CodeAnalysis;
using FamilyHubs.Referral.Core.ApiClients;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;

namespace FamilyHubs.Referral.Api;

public static class StartupExtensions
{
    public static void ConfigureHost(this WebApplicationBuilder builder)
    {
        // ApplicationInsights
        builder.Host.UseSerilog((_, services, loggerConfiguration) =>
        {
            var logLevelString = builder.Configuration["LogLevel"];

            var parsed = Enum.TryParse<LogEventLevel>(logLevelString, out var logLevel);

            loggerConfiguration.WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces,
                parsed ? logLevel : LogEventLevel.Warning);

            loggerConfiguration.WriteTo.Console(
                parsed ? logLevel : LogEventLevel.Warning);
        });
    }

    public static void RegisterApplicationComponents(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationPolicy(configuration);

        services.AddBearerAuthentication(configuration);

        services.RegisterAppDbContext(configuration);

        services.RegisterMinimalEndPoints();

        services.RegisterAutoMapper();

        services.RegisterMediator();
    }

    private static void AddAuthorizationPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        List<string> userRoles = new List<string>()
        {
             RoleTypes.DfeAdmin, RoleTypes.LaProfessional, RoleTypes.VcsManager, RoleTypes.VcsProfessional, RoleTypes.VcsDualRole
        };

        string settingUserRoles = configuration.GetValue<string>("UserRoles") ?? string.Empty;
        if (!string.IsNullOrEmpty(settingUserRoles)) 
        {
            userRoles = settingUserRoles.Split(',').ToList();
        }

        services.AddAuthorization(options => {
            options.AddPolicy("ReferralUser", policy =>
                policy.RequireAssertion(context =>
                    userRoles.Any(role => context.User.IsInRole(role))));
        });
    }

    private static void RegisterMinimalEndPoints(this IServiceCollection services)
    {
        services.AddTransient<MinimalGeneralEndPoints>();
        services.AddTransient<MinimalReferralEndPoints>();
        services.AddTransient<MinimalUserAccountEndPoints>();
    }

    private static void RegisterAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper((serviceProvider, cfg) =>
        {
            var auditProperties = new[] { "Created", "CreatedBy", "LastModified", "LastModifiedBy" };
            cfg.AddProfile<AutoMappingProfiles>();
            cfg.AddCollectionMappers();
            cfg.UseEntityFrameworkCoreModel<ApplicationDbContext>(serviceProvider);
            cfg.ShouldMapProperty = pi => !auditProperties.Contains(pi.Name);
        }, typeof(AutoMappingProfiles));
    }

    private static void RegisterAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<AuditableEntitySaveChangesInterceptor>();
        services.AddTransient<ApplicationDbContextInitialiser>();

        var connectionString = configuration.GetConnectionString("ReferralConnection");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        var useSqlite = configuration.GetValue<bool?>("UseSqlite");
        ArgumentNullException.ThrowIfNull(useSqlite);

        //DO not remove, This will prevent Application from starting if wrong type of connection string is provided
        var connection = (useSqlite == true)
            ? new SqliteConnectionStringBuilder(connectionString).ToString()
            : new SqlConnectionStringBuilder(connectionString).ToString();

        // Register Entity Framework
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            if (useSqlite == true)
            {
                options.UseSqlite(connection, mg =>
                    mg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString())
                        .MigrationsHistoryTable("ReferralMigrationHistory"));  
            }
            else
            {
                options.UseSqlServer(connection, mg =>
                    mg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString())
                        .MigrationsHistoryTable("ReferralMigrationHistory"));
            }
        });
    }

    public static void RegisterMediator(this IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(CreateReferralCommand).Assembly
        };

        services.AddMediatR(config =>
        {
            config.Lifetime = ServiceLifetime.Transient;
            config.RegisterServicesFromAssemblies(assemblies);
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddTransient<CorrelationMiddleware>();
        services.AddTransient<ExceptionHandlingMiddleware>();
    }

    public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);

        var delay = Backoff.DecorrelatedJitterBackoffV2(
            medianFirstRetryDelay: TimeSpan.FromSeconds(1),
            retryCount: 2);

        services.AddHttpClient<INotificationClientService, NotificationClientService>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("NotificationUrl")!);

            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()
                                      ?? throw new ArgumentException($"IHttpContextAccessor required for {nameof(AddHttpClients)}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {httpContextAccessor.HttpContext!.GetBearerToken()}");
        })
            .AddPolicyHandler((callbackServices, request) => HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(delay, (result, timespan, retryAttempt, context) =>
                {
                    callbackServices.GetService<ILogger<NotificationClientService>>()?
                        .LogWarning("Delaying for {Timespan}, then making retry {RetryAttempt}.",
                            timespan, retryAttempt);
                }))
            .AddPolicyHandler(timeoutPolicy);
    }

    public static IServiceCollection AddSecuredTypedHttpClient<TClient, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
            this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient)
            where TClient : class
            where TImplementation : class, TClient
    {
        services.AddHttpClient<TClient, TImplementation>((serviceProvider, httpClient) =>
        {
            configureClient(serviceProvider, httpClient);
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            if (httpContextAccessor == null)
                throw new ArgumentException($"IHttpContextAccessor required for {nameof(AddSecuredTypedHttpClient)}");

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {httpContextAccessor.HttpContext!.GetBearerToken()}");

        });

        return services;
    }

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        services.AddApplicationInsightsTelemetry();
        // Add services to the container.
        services.AddHttpClients(configuration);

        // Add services to the container.
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FamilyHubs.Referral.Api", Version = "v1" });
            c.EnableAnnotations();
        });
    }

    public static async Task ConfigureWebApplication(this WebApplication webApplication)
    {
        webApplication.UseSerilogRequestLogging();

        webApplication.UseMiddleware<CorrelationMiddleware>();
        webApplication.UseMiddleware<ExceptionHandlingMiddleware>();

        // Configure the HTTP request pipeline.
        webApplication.UseSwagger();
        webApplication.UseSwaggerUI();

        webApplication.UseHttpsRedirection();

        webApplication.MapControllers();

        await RegisterEndPoints(webApplication);
    }

    private static async Task RegisterEndPoints(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var genapi = scope.ServiceProvider.GetService<MinimalGeneralEndPoints>();
        genapi?.RegisterMinimalGeneralEndPoints(app);

        var referralApi = scope.ServiceProvider.GetService<MinimalReferralEndPoints>();
        referralApi?.RegisterReferralEndPoints(app);

        var userAccountsApi = scope.ServiceProvider.GetService<MinimalUserAccountEndPoints>();
        userAccountsApi?.RegisterUserAccountEndPoints(app);

        try
        {
            if (!app.Environment.IsProduction())
            {
                // Seed Database
                // Seed Database
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                var shouldRestDatabaseOnRestart = app.Configuration.GetValue<bool>("ShouldRestDatabaseOnRestart");
                await initialiser.InitialiseAsync(app.Environment.IsProduction(), shouldRestDatabaseOnRestart);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
        }
    }
}