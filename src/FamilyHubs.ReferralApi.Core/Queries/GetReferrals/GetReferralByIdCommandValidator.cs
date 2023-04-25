using FluentValidation;

namespace FamilyHubs.ReferralApi.Core.Queries.GetReferrals;

public class GetReferralByIdCommandValidator : AbstractValidator<GetReferralByIdCommand>
{
    public GetReferralByIdCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotNull()
            .NotEmpty();
    }

}
