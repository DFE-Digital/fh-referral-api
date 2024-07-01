using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FamilyHubs.ReferralService.Shared.CreateUpdateDto;
using FamilyHubs.ReferralService.Shared.Dto;
using FluentAssertions;

namespace FamilyHubs.Referral.UnitTests;

public class WhenValidatingReferralCommands
{
    [Fact]
    public void ThenShouldNotErrorWhenCreateReferralCommandModelIsValid()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        testReferral.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(123L));

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral);

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
        var createReferral = new CreateReferralDto(null!, new ConnectionRequestsSentMetricDto(123L));

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral);

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
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(123L));

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral);

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
        var testModel = new CreateReferralCommand(createReferral);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldErrorWhenOrganisationIdIsZero()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        testReferral.Id = 0;
        var createReferral = new CreateReferralDto(testReferral, new ConnectionRequestsSentMetricDto(0L));

        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(createReferral);

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
    public void ThenShouldErrorWhenUpdateReferralCommandModelIsValidButRefferIdIsZero()
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
