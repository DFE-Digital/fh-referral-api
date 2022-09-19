using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Interfaces.Events;
using FamilyHubs.SharedKernel;

namespace FamilyHubs.ReferralApi.Core.Events;


public class ReferralUpdatedEvent : DomainEventBase, IReferralCreatedEvent
{
    public ReferralUpdatedEvent(Referral item)
    {
        Item = item;
    }

    public Referral Item { get; }
}

