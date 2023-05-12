using FamilyHubs.Referral.Data.EMailServices;

namespace FamilyHubs.Referral.Core.Interfaces.Commands;

public interface ICreateNotificationCommand
{
    MessageDto MessageDto { get; }
}
