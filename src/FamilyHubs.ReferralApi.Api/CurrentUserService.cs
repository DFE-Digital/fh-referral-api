using FamilyHubs.ReferralApi.Core;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace FamilyHubs.ReferralApi.Api;

[ExcludeFromCodeCoverage]
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
