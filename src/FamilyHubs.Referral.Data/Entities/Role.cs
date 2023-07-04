namespace FamilyHubs.Referral.Data.Entities;

public class Role : EntityBase<long>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
