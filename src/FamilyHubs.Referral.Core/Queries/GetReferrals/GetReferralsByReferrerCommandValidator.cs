using FluentValidation;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralsByReferrerCommandValidator : AbstractValidator<GetReferralsByReferrerCommand>
{
    public GetReferralsByReferrerCommandValidator()
    {
        RuleFor(v => v.EmailAddress)
            .NotNull()
            .NotEmpty();
    }

}
