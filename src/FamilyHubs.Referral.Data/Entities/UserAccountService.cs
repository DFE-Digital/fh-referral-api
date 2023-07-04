using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class UserAccountService : EntityBase<long>
{
    public long UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public virtual required UserAccount UserAccount { get; set; }

    public long ReferralServiceId { get; set; }
    [ForeignKey("ReferralServiceId")]
    public virtual required ReferralService ReferralService { get; set; }

    
}
