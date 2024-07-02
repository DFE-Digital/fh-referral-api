using FamilyHubs.Referral.Api.Commands.SetReferralStatus;
using FamilyHubs.Referral.Core.Commands.SetReferralStatus;
using FamilyHubs.SharedKernel.Identity;
using FluentAssertions;

namespace FamilyHubs.Referral.UnitTests;

public class WhenValidatingReferralStatusCommand
{
    [Fact]
    public void ThenShouldNotErrorWhenSetReferralStatusCommandModelIsValid()
    {
        //Arrange
        var validator = new SetReferralStatusCommandValidator();
        var testModel = new SetReferralStatusCommand(RoleTypes.VcsProfessional, 1, 1, "New", default!);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeFalse();
    }

    [Fact]
    public void ThenShouldErrorWhenSetReferralStatusCommandModelRoleIsMissingIsNotValid()
    {
        //Arrange
        var validator = new SetReferralStatusCommandValidator();
        var testModel = new SetReferralStatusCommand(default!, 1, 1, "New", default!);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldErrorWhenSetReferralStatusCommandModelOrganisationIdIsMissingIsNotValid()
    {
        //Arrange
        var validator = new SetReferralStatusCommandValidator();
        var testModel = new SetReferralStatusCommand(RoleTypes.VcsProfessional, 0, 1, "New", default!);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldErrorWhenSetReferralStatusCommandModelReferralIdIsMissingIsNotValid()
    {
        //Arrange
        var validator = new SetReferralStatusCommandValidator();
        var testModel = new SetReferralStatusCommand(RoleTypes.VcsProfessional, 1, 0, "New", default!);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }

    [Fact]
    public void ThenShouldErrorWhenSetReferralStatusCommandModelStatusIsMissingIsNotValid()
    {
        //Arrange
        var validator = new SetReferralStatusCommandValidator();
        var testModel = new SetReferralStatusCommand(RoleTypes.VcsProfessional, 1, 0, default!, default!);

        //Act
        var result = validator.Validate(testModel);

        //Assert
        result.Errors.Any().Should().BeTrue();
    }
}
