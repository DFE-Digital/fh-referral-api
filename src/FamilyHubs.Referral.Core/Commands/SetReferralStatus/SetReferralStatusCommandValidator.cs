using FamilyHubs.Referral.Core.Commands.SetReferralStatus;
using FluentValidation;

namespace FamilyHubs.Referral.Api.Commands.SetReferralStatus;


public class SetReferralStatusCommandValidator : AbstractValidator<SetReferralStatusCommand>
{
    public SetReferralStatusCommandValidator()
    {
      
        RuleFor(v => v.ReferralId)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.Status)
            .MinimumLength(1)
            .MaximumLength(50)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.Role)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.UserOrganisationId)
            .GreaterThan(0);
    }
}
