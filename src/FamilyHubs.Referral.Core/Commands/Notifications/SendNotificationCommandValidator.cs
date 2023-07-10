using FamilyHubs.Referral.Core.Commands.Notifications;
using FluentValidation;

namespace FamilyHubs.Referral.Core.Commands.CreateReferral;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(v => v.ReferralId)
            .GreaterThan(0);
    }
}
