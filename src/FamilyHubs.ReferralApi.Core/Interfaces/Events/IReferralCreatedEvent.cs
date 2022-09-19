using FamilyHubs.ReferralApi.Core.Entities;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Events;

public interface IReferralCreatedEvent
{
    Referral Item { get; }
}
