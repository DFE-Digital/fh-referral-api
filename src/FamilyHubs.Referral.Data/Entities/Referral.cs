using EncryptColumn.Core.Attribute;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class Referral : EntityBase<long>
{
    public string? ReferrerTelephone { get; set; }
    public required string ReasonForSupport { get; set; }
    public required string EngageWithFamily { get; set; }
    public string? ReasonForDecliningSupport { get; set; } 
    public long StatusId { get; set; }
    [ForeignKey("StatusId")]
    public virtual required ReferralStatus Status { get; set; }
    public long RecipientId { get; set; }
    [ForeignKey("RecipientId")]
    public virtual required Recipient Recipient { get; set; }
    public long ReferrerId { get; set; }
    [ForeignKey("ReferrerId")]
    public virtual required ReferralUserAccount ReferralUserAccount { get; set; }

    public long ReferralServiceId { get; set; }
    [ForeignKey("ReferralServiceId")]
    public virtual required ReferralService ReferralService { get; set; }

}
