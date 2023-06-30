namespace FamilyHubs.Referral.Data.Entities;

public class ReferralOrganisation : EntityBase<long>
{
    public long? ReferralServiceId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
