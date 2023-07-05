using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
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
        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(testReferral);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldErrorWhenReferrerIdIsZero()
    {
        //Arrange
        var testReferral = WhenUsingReferralCommands.GetReferralDto();
        testReferral.Id = 0;
        var validator = new CreateReferralCommandValidator();
        var testModel = new CreateReferralCommand(testReferral);
        testReferral.ReferralUserAccountDto.Id = 0;

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
