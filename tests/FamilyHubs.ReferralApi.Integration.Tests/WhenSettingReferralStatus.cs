using FamilyHubs.Referral.Core.Commands.SetReferralStatus;
using FamilyHubs.SharedKernel.Identity;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.Integration.Tests;

public class WhenSettingReferralStatus : DataIntegrationTestBase
{
    [Fact]
    public async Task ThenUpdateReferralStatusToDeclined()
    {
        await CreateReferral();
        var testReferral = TestDataProvider.GetReferralDto();
        SetReferralStatusCommand command = new(RoleTypes.VcsProfessional, testReferral.ReferralServiceDto.OrganisationDto.Id, testReferral.Id, "Declined", "Unable to help");
        SetReferralStatusCommandHandler handler = new(TestDbContext, new Mock<ILogger<SetReferralStatusCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new CancellationToken());

        //Assert
        result.Should().Be("Declined");
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == testReferral.Id);
        actualService.Should().NotBeNull();
        actualService!.ReasonForDecliningSupport.Should().Be("Unable to help");
    }
}
