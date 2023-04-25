using FluentValidation;

namespace FamilyHubs.ReferralApi.Core.Queries.GetReferrals;

public class GetReferralsByReferrerCommandValidator : AbstractValidator<GetReferralsByReferrerCommand>
{
    public GetReferralsByReferrerCommandValidator()
    {
        RuleFor(v => v.EmailAddress)
            .NotNull()
            .NotEmpty();
    }

}
