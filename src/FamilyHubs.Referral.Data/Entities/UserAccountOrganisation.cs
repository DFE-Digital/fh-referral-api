using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class UserAccountOrganisation : EntityBase<long>
{
    public long UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public virtual UserAccount UserAccount { get; set; } = null!;

    public long OrganisationId { get; set; }
    [ForeignKey("OrganisationId")]
    public virtual Organisation Organisation { get; set; } = null!;
}
