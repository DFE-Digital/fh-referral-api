using FamilyHubs.ReferralApi.Api.Commands.SetReferralStatus;
using FamilyHubs.ReferralApi.Core.Commands.CreateReferral;
using FamilyHubs.ReferralApi.Core.Commands.SetReferralStatus;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralApi.UnitTests;

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
