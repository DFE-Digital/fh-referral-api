using System.Security.Claims;
using FamilyHubs.Referral.Api.AcceptanceTests.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace FamilyHubs.Referral.Api.AcceptanceTests.Fixtures;

public class BearerTokenGenerator
{
    private readonly string _bearerTokenSigningKey;

    public BearerTokenGenerator()
    {
        ConfigModel config = ConfigAccessor.GetApplicationConfiguration();
        _bearerTokenSigningKey = config.BearerTokenSigningKey;
    }

    public string CreateBearerToken(string role)
    {
        List<Claim> claims = new List<Claim> { new("role", role), new( "OrganisationId", "6"), new("AccountId", "3") };        ClaimsIdentity identity = new ClaimsIdentity(claims, "Test");
        ClaimsPrincipal user = new ClaimsPrincipal(identity);

        SymmetricSecurityKey key =
            new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_bearerTokenSigningKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        JwtSecurityToken token = new JwtSecurityToken(
            claims: user.Claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(5)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}