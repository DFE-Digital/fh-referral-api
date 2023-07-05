using FluentValidation;

namespace FamilyHubs.Referral.Core.Commands.UpdateReferral;

public class UpdateReferralCommandValidator : AbstractValidator<UpdateReferralCommand>
{
    public UpdateReferralCommandValidator()
    {

        RuleFor(v => v.ReferralDto)
            .NotNull();

        RuleFor(v => v.ReferralDto.Id)
            .GreaterThan(0);

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
