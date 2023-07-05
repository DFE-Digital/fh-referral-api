using FluentValidation;

namespace FamilyHubs.Referral.Core.Commands.CreateReferral;

public class CreateReferralCommandValidator : AbstractValidator<CreateReferralCommand>
{
    public CreateReferralCommandValidator()
    {
        RuleFor(v => v.ReferralDto)
            .NotNull();

        RuleFor(v => v.ReferralDto.Id)
            .Equal(0);

        RuleFor(v => v.ReferralDto.ReferralServiceDto)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.ReferralDto.RecipientDto)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.ReferralDto.ReferralUserAccountDto)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.ReferralDto.ReferralUserAccountDto.Id)
            .GreaterThan(0);
    }
}
