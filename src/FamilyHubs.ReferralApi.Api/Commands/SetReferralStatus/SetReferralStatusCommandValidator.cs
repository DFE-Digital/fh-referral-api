using FamilyHubs.ReferralApi.Api.Commands.CreateReferral;
using FluentValidation;

namespace FamilyHubs.ReferralApi.Api.Commands.SetReferralStatus;


public class SetReferralStatusCommandValidator : AbstractValidator<SetReferralStatusCommand>
{
    public SetReferralStatusCommandValidator()
    {
      
        RuleFor(v => v.ReferralId)
            .MinimumLength(1)
            .MaximumLength(50)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.Status)
            .MinimumLength(1)
            .MaximumLength(50)
            .NotNull()
            .NotEmpty();
    }
}
