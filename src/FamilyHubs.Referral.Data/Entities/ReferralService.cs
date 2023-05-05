using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class ReferralService : EntityBase<long>
{
    public required long ReferralId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public virtual required ReferralOrganisation ReferralOrganisation { get; set; }
}
