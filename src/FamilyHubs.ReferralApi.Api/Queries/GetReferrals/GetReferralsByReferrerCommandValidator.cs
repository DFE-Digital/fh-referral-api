using FluentValidation;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralsByReferrerCommandValidator : AbstractValidator<GetReferralsByReferrerCommand>
{
    public GetReferralsByReferrerCommandValidator()
    {
        RuleFor(v => v.EmailAddress)
            .NotNull()
            .NotEmpty();
    }

}
