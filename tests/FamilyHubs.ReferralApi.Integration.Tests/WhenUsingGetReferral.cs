using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FluentAssertions;

namespace FamilyHubs.Referral.Integration.Tests;

public class WhenUsingGetReferral : DataIntegrationTestBase
{
    [Fact]
    public async Task ThenGetReferralByIdOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        
        GetReferralByIdCommand command = new(2);
        GetReferralByIdCommandHandler handler = new(TestDbContext, Mapper);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(referral);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();

    }

    [Fact]
    public async Task ThenGetReferralsByOrganisationIdOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();

        GetReferralsByOrganisationIdCommand command = new(2,1,10);
        GetReferralsByOrganisationIdCommandHandler handler = new(TestDbContext, Mapper);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Items[0].Should().BeEquivalentTo(referral);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();

    }

    [Fact]
    public async Task ThenGetReferralsByReferrerOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();

        GetReferralsByReferrerCommand command = new(referral.ReferrerDto.EmailAddress, 1, 10);
        GetReferralsByReferrerCommandHandler handler = new(TestDbContext, Mapper);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Items[0].Should().BeEquivalentTo(referral);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();

    }
}
