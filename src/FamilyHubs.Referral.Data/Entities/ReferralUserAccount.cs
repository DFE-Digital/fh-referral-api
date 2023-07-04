namespace FamilyHubs.Referral.Data.Entities;

public class ReferralUserAccount : EntityBase<long>
{
    public required string EmailAddress { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public virtual IList<UserAccountRole>? UserAccountRoles { get; set; }
    public string? Team { get; set; }
}
