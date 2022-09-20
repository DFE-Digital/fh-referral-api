using Microsoft.AspNetCore.Mvc.Testing;

namespace FamilyHubs.ReferralApi.FunctionalTests;

public abstract class BaseWhenUsingOpenReferralApiUnitTests
{
    protected readonly HttpClient _client;

    public BaseWhenUsingOpenReferralApiUnitTests()
    {
        var webAppFactory = new WebApplicationFactory<Program>();

        _client = webAppFactory.CreateDefaultClient();
        _client.BaseAddress = new Uri("https://localhost:7128/");

    }
}
