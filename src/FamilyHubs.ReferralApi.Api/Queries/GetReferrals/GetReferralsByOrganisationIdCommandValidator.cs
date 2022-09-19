using FluentValidation;

namespace FamilyHubs.ReferralApi.Api.Queries.GetReferrals;

public class GetReferralsByOrganisationIdCommandValidator : AbstractValidator<GetReferralsByOrganisationIdCommand>
{
    public GetReferralsByOrganisationIdCommandValidator()
    {
        RuleFor(v => v.OrganisationId)
            .NotNull()
            .NotEmpty();
    }
}
