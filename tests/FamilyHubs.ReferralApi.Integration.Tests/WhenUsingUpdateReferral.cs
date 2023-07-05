using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FamilyHubs.Referral.Integration.Tests;

public class WhenUsingUpdateReferral : DataIntegrationTestBase
{
    [Fact]
    public async Task ThenUpdateReferralOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        referral.ReasonForSupport = "Unit Test Reason For Support";
        referral.EngageWithFamily = "Unit Test Engage With Family";

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.ReferrerTelephone.Should().Be(referral.ReferrerTelephone);
        actualService!.ReasonForSupport.Should().Be(referral.ReasonForSupport);
        actualService!.EngageWithFamily.Should().Be(referral.EngageWithFamily);

    }

    [Fact]
    public async Task ThenUpdateStatusOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        referral.Status = new ReferralService.Shared.Dto.ReferralStatusDto
        {
            Id = 2,
            Name = "Opened",
            SortOrder = 1
        };

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.Status.Id.Should().Be(referral.Status.Id);
    }

    [Fact]
    public async Task ThenUpdateRecipientOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        referral.RecipientDto.AddressLine1 = "Unit Test Address Line 1";
        referral.RecipientDto.AddressLine2 = "Unit Test Address Line 2";
        referral.RecipientDto.TownOrCity = "Unit Test Town of City";
        referral.RecipientDto.PostCode = "12345";
        referral.RecipientDto.TextPhone = "1234567890";
        referral.RecipientDto.Telephone = "1234567890";
        referral.RecipientDto.Email = "someone@email.com";
        var expected = referral.RecipientDto;

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.Recipient.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ThenUpdateReferrerOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        referral.ReferralUserAccountDto.EmailAddress = "unittestsomeone@email.com";
        var expected = referral.ReferralUserAccountDto;

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.UserAccount.EmailAddress.Should().Be(expected.EmailAddress);
    }

    [Fact]
    public async Task ThenUpdateServiceAndOrganisationOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        referral.ReferralServiceDto.Name = "Unit Test Name";
        referral.ReferralServiceDto.Description = "Unit Test Description";
        referral.ReferralServiceDto.OrganisationDto.Name = "Unit Test Org Name";
        referral.ReferralServiceDto.OrganisationDto.Description = "Unit Test Org Description";
        var expected = referral.ReferralServiceDto;

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.ReferralService.Name.Should().Be(expected.Name);
        actualService!.ReferralService.Description.Should().Be(expected.Description);
        actualService!.ReferralService.Organisation.Name.Should().Be(expected.OrganisationDto.Name);
        actualService!.ReferralService.Organisation.Description.Should().Be(expected.OrganisationDto.Description);
    }


    
}
