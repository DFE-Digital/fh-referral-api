using FluentValidation;

namespace FamilyHubs.Referral.Core.Queries.GetReferrals;

public class GetReferralsByRecipientValidator : AbstractValidator<GetReferralsByRecipientCommand>
{
    public GetReferralsByRecipientValidator()
    {
        RuleFor(request => request.Email)
            .NotNull()
            .NotEmpty()
            .When(request =>
                string.IsNullOrEmpty(request.Telephone) &&
                string.IsNullOrEmpty(request.Textphone) &&
                string.IsNullOrEmpty(request.Name) &&
                string.IsNullOrEmpty(request.Postcode))
            .WithMessage("At least one of Email, Telephone, Textphone, Name, or Postcode must be provided.");

        RuleFor(request => request.Telephone)
            .NotNull()
            .NotEmpty()
            .When(request =>
                string.IsNullOrEmpty(request.Email) &&
                string.IsNullOrEmpty(request.Textphone) &&
                string.IsNullOrEmpty(request.Name) &&
                string.IsNullOrEmpty(request.Postcode))
            .WithMessage("At least one of Email, Telephone, Textphone, Name, or Postcode must be provided.");

        RuleFor(request => request.Textphone)
            .NotNull()
            .NotEmpty()
            .When(request =>
                string.IsNullOrEmpty(request.Email) &&
                string.IsNullOrEmpty(request.Telephone) &&
                string.IsNullOrEmpty(request.Name) &&
                string.IsNullOrEmpty(request.Postcode))
            .WithMessage("At least one of Email, Telephone, Textphone, Name, or Postcode must be provided.");

        RuleFor(request => request.Name)
            .NotNull()
            .NotEmpty()
            .When(request =>
                string.IsNullOrEmpty(request.Email) &&
                string.IsNullOrEmpty(request.Telephone) &&
                string.IsNullOrEmpty(request.Textphone) &&
                string.IsNullOrEmpty(request.Postcode))
            .WithMessage("At least one of Email, Telephone, Textphone, Name, or Postcode must be provided.");

        RuleFor(request => request.Postcode)
            .NotNull()
            .NotEmpty()
            .When(request =>
                string.IsNullOrEmpty(request.Email) &&
                string.IsNullOrEmpty(request.Telephone) &&
                string.IsNullOrEmpty(request.Textphone) &&
                string.IsNullOrEmpty(request.Name))
            .WithMessage("At least one of Email, Telephone, Textphone, Name, or Postcode must be provided.");
    }

}
