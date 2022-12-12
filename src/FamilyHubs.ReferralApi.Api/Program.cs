using FamilyHubs.ReferralApi.Api;
using FamilyHubs.ReferralApi.Api.Endpoints;
using FamilyHubs.ReferralApi.Core.Infrastructure;
using FamilyHubs.ReferralApi.Infrastructure;
using FamilyHubs.ServiceDirectory.Shared.Extensions;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using MassTransit;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

var builder = WebApplication.CreateBuilder(args);
RegisterComponents(builder.Services, builder.Configuration);
builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"))
    };
});

//https://www.youtube.com/watch?v=cbtK3U2aOlg

builder.Services.AddAuthorization(options =>
{
    if (builder.Environment.IsProduction())
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
                .AddInfrastructureServices(builder.Configuration)
                .AddApplicationServices();

builder.Services.AddTransient<MinimalGeneralEndPoints>();
builder.Services.AddTransient<MinimalReferralEndPoints>();

builder.Services.AddSwaggerGen();



if (builder.Configuration.GetValue<bool>("UseRabbitMQ"))
{
    var rabbitMqSettings = builder.Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
    builder.Services.AddMassTransit(mt =>
            mt.UsingRabbitMq((cntxt, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Uri, "/", c =>
                {
                    c.Username(rabbitMqSettings.UserName);
                    c.Password(rabbitMqSettings.Password);
                });

                cfg.ReceiveEndpoint("referralqueue", (c) =>
                {
                    c.Consumer<CommandMessageConsumer>();
                });
            }));
}

var app = builder.Build();

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

using (var scope = app.Services.CreateScope())
{
    var genapi = scope.ServiceProvider.GetService<MinimalGeneralEndPoints>();
    if (genapi != null)
        genapi.RegisterMinimalGeneralEndPoints(app);

    var referralApi = scope.ServiceProvider.GetService<MinimalReferralEndPoints>();
    if (referralApi != null)
        referralApi.RegisterReferralEndPoints(app);

    try
    {
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        if (!builder.Environment.IsProduction())
        {
            // Seed Database
            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.InitialiseAsync(builder.Configuration);
            await initialiser.SeedAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        if (logger != null)
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
}

Program.ServiceProvider = app.Services;

app.Run();

static void RegisterComponents(IServiceCollection builder, IConfiguration configuration)
{
    builder.AddApplicationInsights(configuration, "fh_referral_api.api");
}
#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
{
    public static IServiceProvider ServiceProvider { get; set; } = default!;
}