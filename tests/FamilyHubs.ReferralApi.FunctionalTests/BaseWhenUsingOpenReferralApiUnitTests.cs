using FamilyHubs.Referral.Api;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
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
    protected HttpClient? Client;
    protected readonly CustomWebApplicationFactory? _webAppFactory;
    private bool _disposed;
    protected readonly JwtSecurityToken? _token;
    protected readonly JwtSecurityToken? _token_forOrganisation1;
    protected readonly JwtSecurityToken? _vcstoken;
    protected readonly JwtSecurityToken? _forbiddentoken;
    private readonly IConfiguration? _configuration;
    private readonly bool _initSuccessful;

    protected BaseWhenUsingOpenReferralApiUnitTests()
    {
        _disposed = false;

        try
        {
         var configuration = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

        _configuration = configuration;

        var jti = Guid.NewGuid().ToString();
        var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration["GovUkOidcConfiguration:BearerTokenSigningKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        _token = new JwtSecurityToken(
            claims: new List<Claim>
                {
        new Claim("sub", configuration["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
        new Claim("jti", jti),
        new Claim(ClaimTypes.Role, RoleTypes.LaProfessional),
        new Claim(FamilyHubsClaimTypes.OrganisationId, "1")

                },
            signingCredentials: creds,
        expires: DateTime.UtcNow.AddMinutes(5)
            );

        _token_forOrganisation1 = new JwtSecurityToken(
            claims: new List<Claim>
                {
        new Claim("sub", configuration["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
        new Claim("jti", jti),
        new Claim("AccountId", "5"),
        new Claim(ClaimTypes.Role, RoleTypes.LaProfessional),
        new Claim(FamilyHubsClaimTypes.OrganisationId, "1")

                },
            signingCredentials: creds,
        expires: DateTime.UtcNow.AddMinutes(5)
            );

            _vcstoken = new JwtSecurityToken(
            claims: new List<Claim>
                {
        new Claim("sub", configuration["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
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
        new Claim("sub", configuration["GovUkOidcConfiguration:Oidc:ClientId"] ?? ""),
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

            _initSuccessful = true;
        }
        catch
        {
            _initSuccessful = false;
        }
    }

    public static UserAccountDto GetUserAccount()
    {
        UserAccountDto userAccountDto = new UserAccountDto
        {
            Id = 2,
            EmailAddress = "FirstUser@email.com",
            Name = "First User",
            PhoneNumber = "0161 111 1111",
            Team = "Test Team"
        };

        userAccountDto.OrganisationUserAccounts = new List<UserAccountOrganisationDto>
        {
            new UserAccountOrganisationDto
            {
                UserAccount = default!,
                Organisation = new OrganisationDto
                {
                    Id = 2,
                    Name = "Organisation",
                    Description = "Organisation Description",
                }
            }
        };

        return userAccountDto;
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
        if (!_initSuccessful) 
        {
            return;
        }

        if (_webAppFactory != null)
        {
            using var scope = _webAppFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
        }
        
        if (Client != null)
        {
            Client.Dispose();
        }

        if (_webAppFactory != null)
        {
            _webAppFactory.Dispose();
        }
            
        GC.SuppressFinalize(this);
    }

    protected bool IsRunningLocally()
    {
        
        if (!_initSuccessful || _configuration == null) 
        {
            return false;
        }

        try
        {
            string localMachineName = _configuration["LocalSettings:MachineName"] ?? string.Empty;

            if (!string.IsNullOrEmpty(localMachineName))
            {
                return Environment.MachineName.Equals(localMachineName, StringComparison.OrdinalIgnoreCase);
            }
        }
        catch
        {
            return false;
        }
        
        // Fallback to a default check if User Secrets file or machine name is not specified
        // For example, you can add additional checks or default behavior here
        return false;
    }
}

#pragma warning restore S3881