using FluentValidation;

namespace FamilyHubs.ReferralApi.Core.Queries.GetReferrals;

public class GetReferralsByOrganisationIdCommandValidator : AbstractValidator<GetReferralsByOrganisationIdCommand>
{
    public GetReferralsByOrganisationIdCommandValidator()
    {
        RuleFor(v => v.OrganisationId)
            .NotNull()
            .NotEmpty();
    }
}
