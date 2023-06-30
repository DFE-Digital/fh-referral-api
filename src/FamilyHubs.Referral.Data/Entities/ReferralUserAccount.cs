using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class ReferralUserAccount : EntityBase<long>
{
    public required string EmailAddress { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public string? Team { get; set; }
    public long ReferralOrganisationId { get; set; }
    [ForeignKey("ReferralOrganisationId")]
    public virtual required ReferralOrganisation ReferralOrganisation { get; set; }
}
