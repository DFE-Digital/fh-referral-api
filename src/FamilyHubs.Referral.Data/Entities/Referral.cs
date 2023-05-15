using EncryptColumn.Core.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class Referral : EntityBase<long>
{
    public required string ReasonForSupport { get; set; }
    public required string EngageWithFamily { get; set; }
    public string? ReasonForDecliningSupport { get; set; }
    [ForeignKey("ReferralStatus")]
    public long ReferralStatusId { get; set; }
    public virtual required ReferralStatus Status { get; set; }
    [ForeignKey("Recipient")]
    public long RecipientId { get; set; }
    public virtual required Recipient Recipient { get; set; }
    [ForeignKey("Referrer")]
    public long ReferrerId { get; set; }
    public virtual required Referrer Referrer { get; set; }
    [ForeignKey("ReferralService")]
    public long ReferralServiceId { get; set; }
    public virtual required ReferralService ReferralService { get; set; }

}