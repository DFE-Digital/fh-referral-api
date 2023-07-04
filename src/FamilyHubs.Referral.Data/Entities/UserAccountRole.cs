using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class UserAccountRole : EntityBase<long>
{
    public long UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public virtual required UserAccount UserAccount { get; set; }

    public long RoleId { get; set; }
    [ForeignKey("RoleId")]
    public virtual required Role Role { get; set; }
}
