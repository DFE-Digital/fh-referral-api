using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class OrganisationUserAccount : EntityBase<long>
{
    public long OrganisationId { get; set; }
    [ForeignKey("OrganisationId")]
    public virtual required Organisation Organisation { get; set; }

    public long UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public virtual required UserAccount UserAccount { get; set; }
}
