using FluentValidation;

namespace FamilyHubs.ReferralApi.Core.Commands.UpdateReferral;

public class UpdateReferralCommandValidator : AbstractValidator<UpdateReferralCommand>
{
    public UpdateReferralCommandValidator()
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

        RuleFor(v => v.ReferralDto.ReferrerDto)
            .NotNull()
            .NotEmpty();

    }
}
