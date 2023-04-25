using FamilyHubs.ReferralApi.Core.Commands.SetReferralStatus;
using FluentValidation;

namespace FamilyHubs.ReferralApi.Api.Commands.SetReferralStatus;


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
