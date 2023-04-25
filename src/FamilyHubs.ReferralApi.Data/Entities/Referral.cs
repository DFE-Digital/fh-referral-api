using EncryptColumn.Core.Attribute;

namespace FamilyHubs.ReferralApi.Data.Entities;

public class Referral : EntityBase<long>
{
    public required string ReferenceNumber { get; set; }
    [EncryptColumn]
    public required string ReasonForSupport { get; set; }
    [EncryptColumn]
    public required string EngageWithFamily { get; set; }

    public virtual required Recipient Recipient { get; set; }

    public virtual required Referrer Referrer { get; set; }

    public virtual required ReferralService ReferralService { get; set; }

    public ICollection<ReferralStatus> Status { get; set; } = new List<ReferralStatus>();
}
