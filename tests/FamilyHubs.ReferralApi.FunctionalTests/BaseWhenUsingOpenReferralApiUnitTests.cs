﻿using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.SharedKernel.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FamilyHubs.Referral.FunctionalTests;

#pragma warning disable S3881
public abstract class BaseWhenUsingOpenReferralApiUnitTests : IDisposable
{
    protected readonly HttpClient Client;
    private readonly CustomWebApplicationFactory _webAppFactory;
    private bool _disposed;
    protected readonly JwtSecurityToken _token;
    protected readonly JwtSecurityToken _vcstoken;
    protected readonly JwtSecurityToken _forbiddentoken;

    protected BaseWhenUsingOpenReferralApiUnitTests()
    {
        _disposed = false;

        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                 .AddEnvironmentVariables()
                 .Build();

        var jti = Guid.NewGuid().ToString();
        var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(config["GovUkOidcConfiguration:BearerTokenSigningKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        _token = new JwtSecurityToken(
            claims: new List<Claim>
               {
                    new Claim("sub", config["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
                    new Claim("jti", jti),
                    new Claim(ClaimTypes.Role, RoleTypes.LaProfessional),
                    new Claim(FamilyHubsClaimTypes.OrganisationId, "1")

               },
            signingCredentials: creds,
        expires: DateTime.UtcNow.AddMinutes(5)
            );

        _vcstoken = new JwtSecurityToken(
            claims: new List<Claim>
               {
                    new Claim("sub", config["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
                    new Claim("jti", jti),
                    new Claim(ClaimTypes.Role, RoleTypes.VcsProfessional),
                    new Claim(FamilyHubsClaimTypes.OrganisationId, "1")

               },
            signingCredentials: creds,
        expires: DateTime.UtcNow.AddMinutes(5)
            );

        _forbiddentoken = new JwtSecurityToken(
            claims: new List<Claim>
               {
                    new Claim("sub", config["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
                    new Claim("jti", jti),
                    new Claim(ClaimTypes.Role, RoleTypes.VcsProfessional),
                    new Claim(FamilyHubsClaimTypes.OrganisationId, "-1")

               },
            signingCredentials: creds,
        expires: DateTime.UtcNow.AddMinutes(5)
            );

        _webAppFactory = new CustomWebApplicationFactory();
        _webAppFactory.SetupTestDatabaseAndSeedData();

        Client = _webAppFactory.CreateDefaultClient();
        Client.BaseAddress = new Uri("https://localhost:7192/");
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
        if (!_disposed &&  disposing)
        {
            Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        using var scope = _webAppFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureDeleted();

        Client.Dispose();
        _webAppFactory.Dispose();
        GC.SuppressFinalize(this);
    }
}

#pragma warning restore S3881