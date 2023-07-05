using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class UserAccountService : EntityBase<long>
{
    public long UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public virtual UserAccount UserAccount { get; set; } = null!;

    public long ReferralServiceId { get; set; }
    [ForeignKey("ReferralServiceId")]
    public virtual ReferralService ReferralService { get; set; } = null!;


}
