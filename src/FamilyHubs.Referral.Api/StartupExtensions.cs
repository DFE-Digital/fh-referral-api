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
using FamilyHubs.SharedKernel.Security;
using FamilyHubs.Referral.Core.ClientServices;
using FamilyHubs.SharedKernel.Razor.Health;

namespace FamilyHubs.Referral.Api;

public static class StartupExtensions
{
    public static void ConfigureHost(this WebApplicationBuilder builder)
    {
        // ApplicationInsights
        builder.Host.UseSerilog((_, services, loggerConfiguration) =>
        {
            string? logLevelString = builder.Configuration["LogLevel"];

            if (logLevelString == null) 
            {
                logLevelString = "Information";
            }

            var parsed = Enum.TryParse<LogEventLevel>(logLevelString, out var logLevel);

            loggerConfiguration.WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces,
                parsed ? logLevel : LogEventLevel.Information);

            loggerConfiguration.WriteTo.Console(
                parsed ? logLevel : LogEventLevel.Information);
        });
    }

    public static void RegisterApplicationComponents(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IKeyProvider, KeyProvider>();
        services.AddSingleton<ICrypto, Crypto>();

        var serviceDirectoryApiBaseUrl = configuration["ServiceDirectoryApiBaseUrl"];
        if (string.IsNullOrWhiteSpace(serviceDirectoryApiBaseUrl))
        {
            //todo: config exception
            throw new ArgumentException("ServiceDirectoryApiBaseUrl is not configured");
        }

        services.AddHttpClient<IServiceDirectoryService, ServiceDirectoryService>(client =>
        {
            client.BaseAddress = new Uri(serviceDirectoryApiBaseUrl);
        });

        services.AddAuthorizationPolicy(configuration);

        services.AddBearerAuthentication(configuration);

        services.AddFamilyHubsHealthChecks(configuration);

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
                    userRoles.Exists(role => context.User.IsInRole(role))));
        });
    }

    private static void RegisterMinimalEndPoints(this IServiceCollection services)
    {
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

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        services.AddSingleton<ITelemetryInitializer, ConnectTelemetryPiiRedactor>();
        services.AddApplicationInsightsTelemetry();

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

        webApplication.MapFamilyHubsHealthChecks(typeof(StartupExtensions).Assembly);

        await RegisterEndPoints(webApplication);
    }

    private static async Task RegisterEndPoints(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var referralApi = scope.ServiceProvider.GetService<MinimalReferralEndPoints>();
        if (referralApi == null)
        {
            throw new InvalidOperationException("MinimalReferralEndPoints is not registered");
        }
        referralApi.RegisterReferralEndPoints(app);

        var userAccountsApi = scope.ServiceProvider.GetService<MinimalUserAccountEndPoints>();
        if (userAccountsApi == null)
        {
            throw new InvalidOperationException("MinimalUserAccountEndPoints is not registered");
        }
        userAccountsApi.RegisterUserAccountEndPoints(app);

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
            Log.Error(ex, "An error occurred seeding the DB. {ExceptionMessage}", ex.Message);
        }
    }
}
