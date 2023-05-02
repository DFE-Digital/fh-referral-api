namespace FamilyHubs.Referral.Data.Entities;

public class Referrer : EntityBase<long>
{
    public required long ReferralId { get; set; }
    public required string EmailAddress { get; set; }
}
