using FamilyHubs.ReferralApi.Api.Endpoints;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ReferralApi.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.ApplicationInsights.Extensibility;
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

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        services.AddApplicationInsightsTelemetry();

        // Adding Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Adding Jwt Bearer
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = configuration["JWT:ValidAudience"],
                ValidIssuer = configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    configuration["JWT:Secret"] ?? "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"))
            };
        });

        //https://www.youtube.com/watch?v=cbtK3U2aOlg
        services.AddAuthorization(options =>
        {
            if (isProduction)
            {
                options.AddPolicy("Referrer", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("VCSAdmin") ||
                        context.User.IsInRole("Professional")));
            }
            else //LocalHost, Dev, Test, PP, disable Authorisation
            {
                options.AddPolicy("Referrer", policy =>
                    policy.RequireAssertion(_ => true));
            }
        });

        // Add services to the container.
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer()
            .AddInfrastructureServices(configuration)
            .AddApplicationServices();

        services.AddTransient<MinimalGeneralEndPoints>();
        services.AddTransient<MinimalReferralEndPoints>();

        services.AddSwaggerGen();

        if (!configuration.GetValue<bool>("UseRabbitMQ")) return;

        var rabbitMqSettings = configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
        if (rabbitMqSettings != null)
        {
           services.AddMassTransit(mt =>
           mt.UsingRabbitMq((_, cfg) =>
           {
               cfg.Host(rabbitMqSettings.Uri, "/", c =>
               {
                   c.Username(rabbitMqSettings.UserName);
                   c.Password(rabbitMqSettings.Password);
               });

               cfg.ReceiveEndpoint("referralqueue", (c) => { c.Consumer<CommandMessageConsumer>(); });
           }));
        }
       
    }

    public static async Task<IServiceProvider> ConfigureWebApplication(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RegisterEndPoints();

        return app.Services;
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
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                await initialiser.InitialiseAsync(app.Configuration);
                await initialiser.SeedAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
        }
    }
}