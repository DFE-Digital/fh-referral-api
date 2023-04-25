namespace FamilyHubs.ReferralApi.Data.Entities;

public class ReferralOrganisation : EntityBase<long>
{
    public required long ReferralServiceId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
