using FamilyHubs.Referral.Core.ClientServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyHubs.Referral.FunctionalTests;

public class WhenUsingServiceDirectoryApiTests : BaseWhenUsingOpenReferralApiUnitTests
{

    [Fact]
    public async Task ThenGetOrganisationByIdIsRetrieved()
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        using var scope = _webAppFactory!.Services.CreateScope();
        var serviceDirectoryService = scope.ServiceProvider.GetRequiredService<IServiceDirectoryService>();

        var result = await serviceDirectoryService.GetOrganisationById(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);

    }

    [Fact]
    public async Task ThenGetServiceByIdIsRetrieved()
    {
        if (!IsRunningLocally() || Client == null)
        {
            // Skip the test if not running locally
            Assert.True(true, "Test skipped because it is not running locally.");
            return;
        }

        using var scope = _webAppFactory!.Services.CreateScope();
        var serviceDirectoryService = scope.ServiceProvider.GetRequiredService<IServiceDirectoryService>();

        var result = await serviceDirectoryService.GetServiceById(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);

    }
}
