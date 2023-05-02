namespace FamilyHubs.Referral.Data.Entities;

public class Recipient : EntityBase<long>
{
    public required long ReferralId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? TextPhone { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? TownOrCity { get; set; }
    public string? Country { get; set; }
    public string? PostCode { get; set; }
}
