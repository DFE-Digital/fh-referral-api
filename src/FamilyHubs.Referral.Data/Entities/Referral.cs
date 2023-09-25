using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class Referral : EntityBase<long>
{
    public string? ReferrerTelephone { get; set; }
    public required string ReasonForSupport { get; set; }
    public required string EngageWithFamily { get; set; }
    public string? ReasonForDecliningSupport { get; set; } 
    public byte StatusId { get; set; }
    [ForeignKey("StatusId")]
    public virtual required Status Status { get; set; }
    public long RecipientId { get; set; }
    [ForeignKey("RecipientId")]
    public virtual required Recipient Recipient { get; set; }
    public long UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public virtual required UserAccount UserAccount { get; set; }

    public long ReferralServiceId { get; set; }
    [ForeignKey("ReferralServiceId")]
    public virtual required ReferralService ReferralService { get; set; }

}
