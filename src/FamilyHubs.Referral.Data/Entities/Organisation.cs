namespace FamilyHubs.Referral.Data.Entities;

public class Organisation : EntityBase<long>
{
    public required long ReferralServiceId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
