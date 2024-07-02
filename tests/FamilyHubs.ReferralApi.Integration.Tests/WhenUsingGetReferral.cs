using Ardalis.GuardClauses;
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

    [Theory]
    [InlineData("JoeBlog@email.com", default!, "Email")]
    [InlineData("078123456", default!, "Telephone")]
    [InlineData("078123456", default!, "Textphone")]
    [InlineData("Joe Blogs", "B30 2TV", "Name")]
    public async Task ThenGetReferralByRecipient(string value1, string value2, string paraType)
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();

        GetReferralsByRecipientCommand command = default!;
        
        switch(paraType)
        {
            case "Email":
                command = new(referral.ReferralServiceDto.OrganisationDto.Id,value1, null, null, null, null);
                break;

            case "Telephone":
                command = new(referral.ReferralServiceDto.OrganisationDto.Id,null, value1, null, null, null);
                break;

            case "Textphone":
                command = new(referral.ReferralServiceDto.OrganisationDto.Id,null, null, value1, null, null);
                break;

            case "Name":
                command = new(referral.ReferralServiceDto.OrganisationDto.Id,null, null, null, value1, value2);
                break;
        }

        GetReferralsByRecipientHandler handler = new(TestDbContext, Mapper);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());



        //Assert
        result.Should().NotBeNull();
        result[0].Should().BeEquivalentTo(referral,
            options => options.Excluding(x => x.Created).Excluding(x => x.LastModified).Excluding(x => x.Status.SecondrySortOrder));
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();

    }

    [Fact]
    public async Task ThenHandle_WithInvalidData_ReturnsValidationResponse()
    {
        // Arrange
        await CreateReferral();
        GetReferralsByRecipientCommand command = new(TestDataProvider.GetReferralDto().ReferralServiceDto.OrganisationDto.Id, null, null, null, null, null);
        GetReferralsByRecipientHandler handler = new(TestDbContext, Mapper);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();

    }
}
