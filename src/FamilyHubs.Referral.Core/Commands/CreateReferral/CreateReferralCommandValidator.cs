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

        RuleFor(v => v.CreateReferral.Metrics.UserOrganisationId)
            .GreaterThan(0);

        RuleFor(v => v.CreateReferral.Referral)
            .NotNull();

        RuleFor(v => v.CreateReferral.Referral.Id)
            .Equal(0);

        RuleFor(v => v.CreateReferral.Referral.ReferralServiceDto)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.CreateReferral.Referral.RecipientDto)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.CreateReferral.Referral.ReferralUserAccountDto)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.CreateReferral.Referral.ReferralUserAccountDto.Id)
            .GreaterThan(0);
    }
}
