using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Interfaces.Events;
using FamilyHubs.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralApi.Core.Events;


public class ReferralStatusCreatedEvent : DomainEventBase, IReferralStatusCreatedEvent
{
    public ReferralStatusCreatedEvent(ReferralStatus item)
    {
        Item = item;
    }

    public ReferralStatus Item { get; }
}
