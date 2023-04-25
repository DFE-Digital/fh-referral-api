using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.ReferralApi.Core.Entities;

public class ReferralService : EntityBase<long>
{
    public required long ReferralId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    //[ForeignKey("ReferralOrganisation")]
    //public long ReferralOrganisationId { get; set; }
    public virtual required ReferralOrganisation ReferralOrganisation { get; set; }
}
