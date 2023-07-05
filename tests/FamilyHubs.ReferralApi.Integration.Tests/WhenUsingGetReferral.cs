using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.ReferralService.Shared.Enums;
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
        result.Should().BeEquivalentTo(referral,
            options => options.Excluding(x => x.Created).Excluding(x => x.LastModified));
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();

    }

    [Theory]
    [InlineData(ReferralOrderBy.DateSent, true)]
    [InlineData(ReferralOrderBy.DateSent, false)]
    [InlineData(ReferralOrderBy.DateUpdated, true)]
    [InlineData(ReferralOrderBy.DateUpdated, false)]
    [InlineData(ReferralOrderBy.Status, true)]
    [InlineData(ReferralOrderBy.Status, false)]
    [InlineData(ReferralOrderBy.RecipientName, true)]
    [InlineData(ReferralOrderBy.RecipientName, false)]
    [InlineData(ReferralOrderBy.Team, true)]
    [InlineData(ReferralOrderBy.Team, false)]
    [InlineData(ReferralOrderBy.ServiceName, true)]
    [InlineData(ReferralOrderBy.ServiceName, false)]

    public async Task ThenGetReferralsByOrganisationIdOnly(ReferralOrderBy referralOrderBy, bool isAssending)
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();

        GetReferralsByOrganisationIdCommand command = new(2, referralOrderBy, isAssending, false,  1,10);
        GetReferralsByOrganisationIdCommandHandler handler = new(TestDbContext, Mapper);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBeNull();
        result.Items[0].Should().BeEquivalentTo(referral,
            options => options.Excluding(x => x.Created).Excluding(x => x.LastModified).Excluding(x => x.Status.SecondrySortOrder));
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();

    }

    [Theory]
    [InlineData(ReferralOrderBy.DateSent, true)]
    [InlineData(ReferralOrderBy.DateSent, false)]
    [InlineData(ReferralOrderBy.DateUpdated, true)]
    [InlineData(ReferralOrderBy.DateUpdated, false)]
    [InlineData(ReferralOrderBy.Status, true)]
    [InlineData(ReferralOrderBy.Status, false)]
    [InlineData(ReferralOrderBy.RecipientName, true)]
    [InlineData(ReferralOrderBy.RecipientName, false)]
    [InlineData(ReferralOrderBy.Team, true)]
    [InlineData(ReferralOrderBy.Team, false)]
    [InlineData(ReferralOrderBy.ServiceName, true)]
    [InlineData(ReferralOrderBy.ServiceName, false)]
    public async Task ThenGetReferralsByReferrerOnly(ReferralOrderBy referralOrderBy, bool isAssending)
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();

        GetReferralsByReferrerCommand command = new(referral.ReferralUserAccountDto.EmailAddress, referralOrderBy, isAssending, null, 1, 10);
        GetReferralsByReferrerCommandHandler handler = new(TestDbContext, Mapper);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        

        //Assert
        result.Should().NotBeNull();
        result.Items[0].Should().BeEquivalentTo(referral,
            options => options.Excluding(x => x.Created).Excluding(x => x.LastModified).Excluding(x => x.Status.SecondrySortOrder));
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();

    }
}
