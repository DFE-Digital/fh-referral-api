using FluentValidation;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralByIdCommandValidator : AbstractValidator<GetReferralByIdCommand>
{
    public GetReferralByIdCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotNull()
            .NotEmpty();
    }

}
