namespace FamilyHubs.Referral.Data.Entities;

public class ReferralService : EntityBase<long>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public long? OrganisationId { get; set; }
    public virtual Organisation? Organisation { get; set; }
}
