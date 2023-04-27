using FamilyHubs.Referral.Api.Commands.SetReferralStatus;
using FamilyHubs.Referral.Core.Commands.CreateReferral;
using FamilyHubs.Referral.Core.Commands.SetReferralStatus;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.Referral.UnitTests;

public class WhenValidatingReferralStatusCommand
{
    [Fact]
    public void ThenShouldNotErrorWhenSetReferralStatusCommandModelIsValid()
    {
        //Arrange
        var validator = new SetReferralStatusCommandValidator();
        var testModel = new SetReferralStatusCommand(1, "active");

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }
}
