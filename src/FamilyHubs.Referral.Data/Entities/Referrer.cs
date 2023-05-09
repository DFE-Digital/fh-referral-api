namespace FamilyHubs.Referral.Data.Entities;

public class Referrer : EntityBase<long>
{
    public required long ReferralId { get; set; }
    public required string EmailAddress { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public string? Team { get; set; }
}
