namespace FamilyHubs.ReferralApi.Core.Entities;

public class ReferralOrganisation : EntityBase<long>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
