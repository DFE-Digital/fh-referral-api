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
        actualService!.ReasonForSupport.Should().Be(referral.ReasonForSupport);
        actualService!.EngageWithFamily.Should().Be(referral.EngageWithFamily);

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
        referral.UserDto.EmailAddress = "unittestsomeone@email.com";
        var expected = referral.UserDto;

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.Referrer.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ThenUpdateServiceAndOrganisationOnly()
    {
        await CreateReferral();
        var referral = TestDataProvider.GetReferralDto();
        referral.ServiceDto.Name = "Unit Test Name";
        referral.ServiceDto.Description = "Unit Test Description";
        referral.ServiceDto.OrganisationDto.Name = "Unit Test Org Name";
        referral.ServiceDto.OrganisationDto.Description = "Unit Test Org Description";
        var expected = referral.ServiceDto;

        UpdateReferralCommand command = new(referral.Id, referral);
        UpdateReferralCommandHandler handler = new(TestDbContext, Mapper, new Mock<ILogger<UpdateReferralCommandHandler>>().Object);

        //Act
        var result = await handler.Handle(command, new System.Threading.CancellationToken());

        //Assert
        result.Should().NotBe(0);
        result.Should().Be(referral.Id);
        var actualService = TestDbContext.Referrals.SingleOrDefault(s => s.Id == referral.Id);
        actualService.Should().NotBeNull();
        actualService!.Service.Name.Should().Be(expected.Name);
        actualService!.Service.Description.Should().Be(expected.Description);
        actualService!.Service.Organisation.Name.Should().Be(expected.OrganisationDto.Name);
        actualService!.Service.Organisation.Description.Should().Be(expected.OrganisationDto.Description);
    }


    
}
