namespace FamilyHubs.Referral.Data.Entities;

public class Role : EntityBase<byte>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
