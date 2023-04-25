﻿using AutoMapper;
using AutoMapper.EquivalencyExpression;
using FamilyHubs.ReferralApi.Core.Commands.CreateReferral;
using FamilyHubs.ReferralApi.Api.Endpoints;
using FamilyHubs.ReferralApi.Api.Middleware;
using FamilyHubs.ReferralApi.Core;
using FamilyHubs.ReferralApi.Data.Interceptors;
using FamilyHubs.ReferralApi.Data.Repository;
using MassTransit;
using MediatR;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace FamilyHubs.ReferralApi.Api;

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
        services.RegisterAppDbContext(configuration);

        services.RegisterMinimalEndPoints();

        services.RegisterAutoMapper();

        services.RegisterMediator();
    }

    private static void RegisterMinimalEndPoints(this IServiceCollection services)
    {
        services.AddTransient<MinimalGeneralEndPoints>();
        services.AddTransient<MinimalReferralEndPoints>();
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
                    mg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString()));
            }
            else
            {
                options.UseSqlServer(connection, mg =>
                    mg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString()));
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

        await RegisterEndPoints(webApplication);
    }

    private static async Task RegisterEndPoints(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var genapi = scope.ServiceProvider.GetService<MinimalGeneralEndPoints>();
        genapi?.RegisterMinimalGeneralEndPoints(app);

        var referralApi = scope.ServiceProvider.GetService<MinimalReferralEndPoints>();
        referralApi?.RegisterReferralEndPoints(app);

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


    //public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    //{
    //    services.AddApplicationInsightsTelemetry();

    //    // Adding Authentication
    //    services.AddAuthentication(options =>
    //    {
    //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //    })
    //    .AddJwtBearer(options =>
    //    {
    //        // Adding Jwt Bearer
    //        options.SaveToken = true;
    //        options.RequireHttpsMetadata = false;
    //        options.TokenValidationParameters = new TokenValidationParameters()
    //        {
    //            ValidateIssuer = true,
    //            ValidateAudience = true,
    //            ValidAudience = configuration["JWT:ValidAudience"],
    //            ValidIssuer = configuration["JWT:ValidIssuer"],
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
    //                configuration["JWT:Secret"] ?? "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"))
    //        };
    //    });

    //    //https://www.youtube.com/watch?v=cbtK3U2aOlg
    //    services.AddAuthorization(options =>
    //    {
    //        if (isProduction)
    //        {
    //            options.AddPolicy("Referrer", policy =>
    //                policy.RequireAssertion(context =>
    //                    context.User.IsInRole("VCSAdmin") ||
    //                    context.User.IsInRole("Professional")));
    //        }
    //        else //LocalHost, Dev, Test, PP, disable Authorisation
    //        {
    //            options.AddPolicy("Referrer", policy =>
    //                policy.RequireAssertion(_ => true));
    //        }
    //    });

    //    // Add services to the container.
    //    services.AddControllers();
    //    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //    services.AddEndpointsApiExplorer()
    //        .AddInfrastructureServices(configuration)
    //        .AddApplicationServices();

    //    services.AddTransient<MinimalGeneralEndPoints>();
    //    services.AddTransient<MinimalReferralEndPoints>();

    //    services.AddSwaggerGen();

    //}

}