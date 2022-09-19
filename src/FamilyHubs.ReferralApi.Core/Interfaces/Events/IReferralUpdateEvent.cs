using FamilyHubs.ReferralApi.Core.Entities;

namespace FamilyHubs.ReferralApi.Core.Interfaces.Events;

public interface IReferralUpdateEvent
{
    public Referral Item { get; }
}
