using FamilyHubs.ReferralService.Shared.Models;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ICreateNotificationCommand
{
    MessageDto MessageDto { get; }
}
