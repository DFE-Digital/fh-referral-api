namespace FamilyHubs.ReferralApi.Core.Entities;

public class Referrer : EntityBase<long>
{
    public required string EmailAddress { get; set; }
}
