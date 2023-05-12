﻿using EncryptColumn.Core.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class Referral : EntityBase<long>
{
    public required string ReasonForSupport { get; set; }
    public required string EngageWithFamily { get; set; }
    public string? ReasonForDecliningSupport { get; set; }

    public virtual required Recipient Recipient { get; set; }

    public virtual required Referrer Referrer { get; set; }

    //public long TeamId { get; set; }
    //[NotMapped]
    public virtual required Team Team { get; set; }

    public virtual required ReferralService ReferralService { get; set; }

    public ICollection<ReferralStatus> Status { get; set; } = new List<ReferralStatus>();
}
