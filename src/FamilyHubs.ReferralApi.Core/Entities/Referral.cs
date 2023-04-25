using EncryptColumn.Core.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.ReferralApi.Core.Entities;

public class Referral : EntityBase<long>
{
    public required string ReferenceNumber { get; set; }
    [EncryptColumn]
    public required string ReasonForSupport { get; set; }
    [EncryptColumn]
    public required string EngageWithFamily { get; set; }

    [ForeignKey("Recipient")]
    public long RecipientId { get; set; }
    public virtual required Recipient Recipient { get; set; }

    [ForeignKey("Referrer")]
    public long ReferrerId { get; set; }
    public virtual required Referrer Referrer { get; set; }

    [ForeignKey("ReferralService")]
    public long ReferralServiceId { get; set; }
    public virtual required ReferralService ReferralService { get; set; }

    public ICollection<ReferralStatus> Status { get; set; } = new List<ReferralStatus>();
}
