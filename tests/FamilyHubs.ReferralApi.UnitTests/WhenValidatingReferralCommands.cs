using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.UpdateReferral;
using FamilyHubs.Referral.Core.Queries.GetReferrals;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        var testModel = new GetReferralsByOrganisationIdCommand(1, 1, 99);

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
        var testModel = new GetReferralsByReferrerCommand("id", 1, 99);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }
}
