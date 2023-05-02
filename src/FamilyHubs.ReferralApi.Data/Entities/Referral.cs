using EncryptColumn.Core.Attribute;

namespace FamilyHubs.Referral.Data.Entities;

public class Referral : EntityBase<long>
{
    public required string ReasonForSupport { get; set; }
    public required string EngageWithFamily { get; set; }

    public virtual required Recipient Recipient { get; set; }

    public virtual required Referrer Referrer { get; set; }

    public virtual required ReferralService ReferralService { get; set; }

    public ICollection<ReferralStatus> Status { get; set; } = new List<ReferralStatus>();
}
