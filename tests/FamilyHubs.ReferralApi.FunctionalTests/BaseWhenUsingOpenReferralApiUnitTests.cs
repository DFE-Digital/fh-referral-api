using FamilyHubs.Referral.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyHubs.Referral.FunctionalTests;

#pragma warning disable S3881
public abstract class BaseWhenUsingOpenReferralApiUnitTests : IDisposable
{
    protected readonly HttpClient Client;
    private readonly CustomWebApplicationFactory _webAppFactory;
    private bool _disposed;
    protected readonly JwtSecurityToken _token;

    protected BaseWhenUsingOpenReferralApiUnitTests()
    {
        _disposed = false;

        var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                 .AddEnvironmentVariables()
                 .Build();

        List<Claim> authClaims = new List<Claim> { new Claim(ClaimTypes.Role, "Professional") };
        _token = CreateToken(authClaims, config);


        _webAppFactory = new CustomWebApplicationFactory();
        _webAppFactory.SetupTestDatabaseAndSeedData();

        Client = _webAppFactory.CreateDefaultClient();
        Client.BaseAddress = new Uri("https://localhost:7192/");
    }

    private JwtSecurityToken CreateToken(List<Claim> authClaims, IConfiguration configuration)
    {
        var secret = configuration["GovUkOidcConfiguration:Oidc:PrivateKey"] ?? "";
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        

        var token = new JwtSecurityToken(
            configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(5),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
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