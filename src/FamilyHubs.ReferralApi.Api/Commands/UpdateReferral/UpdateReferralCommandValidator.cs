using FluentValidation;

namespace FamilyHubs.ReferralApi.Api.Commands.UpdateReferral;

public class UpdateReferralCommandValidator : AbstractValidator<UpdateReferralCommand>
{
    public UpdateReferralCommandValidator()
    {
        RuleFor(v => v.ReferralDto)
            .NotNull();

        RuleFor(v => v.ReferralDto.Id)
            .MinimumLength(1)
            .MaximumLength(50)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.ReferralDto.ServiceId)
            .MaximumLength(50)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.ReferralDto.ServiceName)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.ReferralDto.Referrer)
            .MaximumLength(50)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.ReferralDto.FullName)
            .MinimumLength(1)
            .NotNull()
            .NotEmpty();
    }
}
