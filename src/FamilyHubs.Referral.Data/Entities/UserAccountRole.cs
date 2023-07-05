using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class UserAccountRole : EntityBase<long>
{
    public long UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public virtual UserAccount UserAccount { get; set; } = null!;

    public byte RoleId { get; set; }
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;
}
