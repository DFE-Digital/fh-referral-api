using FluentValidation;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralByIdCommandValidator : AbstractValidator<GetReferralByIdCommand>
{
    public GetReferralByIdCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotNull()
            .NotEmpty();
    }

}
