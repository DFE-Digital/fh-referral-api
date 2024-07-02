using FluentValidation;

namespace FamilyHubs.Referral.Core.Commands.CreateReferral;

public class CreateReferralCommandValidator : AbstractValidator<CreateReferralCommand>
{
    public CreateReferralCommandValidator()
    {
        RuleFor(v => v.CreateReferral)
            .NotNull();

        RuleFor(v => v.CreateReferral.Metrics)
            .NotNull();

        RuleFor(v => v.CreateReferral.Referral)
            .NotNull();

        RuleFor(v => v.CreateReferral.Referral.Id)
            .Equal(0)
            .When(v => v.CreateReferral.Referral != null);

        RuleFor(v => v.CreateReferral.Referral.ReferralServiceDto)
            .NotNull()
            .NotEmpty()
            .When(v => v.CreateReferral.Referral != null);

        RuleFor(v => v.CreateReferral.Referral.RecipientDto)
            .NotNull()
            .NotEmpty()
            .When(v => v.CreateReferral.Referral != null);

        RuleFor(v => v.CreateReferral.Referral.ReferralUserAccountDto)
            .NotNull()
            .NotEmpty()
            .When(v => v.CreateReferral.Referral != null);

        RuleFor(v => v.CreateReferral.Referral.ReferralUserAccountDto.Id)
            .GreaterThan(0)
            .When(v => v.CreateReferral.Referral != null);
    }
}
