using FluentValidation;

namespace FamilyHubs.Referral.Core.Commands.SetReferralStatus;

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
    }
}
