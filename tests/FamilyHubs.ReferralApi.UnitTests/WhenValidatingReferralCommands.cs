using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.ReferralService.Shared.Dto.CreateUpdate;
using FamilyHubs.ReferralService.Shared.Dto.Metrics;
using FamilyHubs.SharedKernel.Identity.Models;
using FluentAssertions;

namespace FamilyHubs.Referral.UnitTests;

public class WhenValidatingReferralCommands
{
    public DateTimeOffset RequestTimestamp { get; set; }
    public FamilyHubsUser FamilyHubsUser { get; set; }
    public const long ExpectedAccountId = 123L;
    public const long ExpectedOrganisationId = 456L;

    public WhenValidatingReferralCommands()
    {
        RequestTimestamp = new DateTimeOffset(new DateTime(2025, 1, 1, 12, 0, 0));

        FamilyHubsUser = new FamilyHubsUser
        {
            AccountId = ExpectedAccountId.ToString(),
            OrganisationId = ExpectedOrganisationId.ToString()
        };
    }

    [Fact]
    public void ThenShouldNotErrorWhenCreateReferralCommandModelIsValid()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        testReferral.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldErrorWhenConnectionReferralIsNull()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        testReferral.Id = 0;
        var createReferral = new CreateReferralDto(null!, new ConnectionRequestsSentMetricDto(RequestTimestamp));

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldErrorWhenReferrerIdIsZero()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        testReferral.Id = 0;
        testReferral.ReferralUserAccountDto.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(RequestTimestamp));

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldErrorWhenConnectionRequestsSentMetricDtoIsNull()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        testReferral.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, null!);

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral, FamilyHubsUser);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldNotErrorWhenUpdateReferralCommandModelIsValid()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        var validator = new UpdateReferralCommandValidator();
        var testModel = new UpdateReferralCommand(testReferral.Id, testReferral);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldErrorWhenUpdateReferralCommandModelIsValidButRefererIdIsZero()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        var validator = new UpdateReferralCommandValidator();
        var testModel = new UpdateReferralCommand(testReferral.Id, testReferral);
        testReferral.ReferralUserAccountDto.Id = 0;

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldNotErrorWhenGetReferralByIdCommandModelIsValid()
    {
        //Arrange
        var validator = new GetReferralByIdCommandValidator();
        var testModel = new GetReferralByIdCommand(1);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldNotErrorWhenGetReferralsByOrganisationIdCommandModelIsValid()
    {
        //Arrange
        var validator = new GetReferralsByOrganisationIdCommandValidator();
        var testModel = new GetReferralsByOrganisationIdCommand(1, null, null, null, 1, 99);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldNotErrorWhenGetReferralsByReferrerCommandModelIsValid()
    {
        //Arrange
        var validator = new GetReferralsByReferrerCommandValidator();
        var testModel = new GetReferralsByReferrerCommand("id", null, null, null, 1, 99);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }
}
