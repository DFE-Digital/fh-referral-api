namespace FamilyHubs.Referral.Data.Entities;

public class User : EntityBase<long>
{
    public required string EmailAddress { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    // A User is associated with one Organisation
    // (Technically a User could be associated with Many Organisations but for this system they would need
    // to have a separate instance with a different email address if they were associated with another Organisation) 
    public required long OrganisationId { get; set; }

    // A User can be a member of zero or many Teams
    public virtual ICollection<Team>? Teams { get; set; }

    // A User can have zero or many Roles
    public virtual ICollection<Role>? Role { get; set; }

    // A Users can be assocated with one or many Referrals
    public virtual ICollection<Referral>? Referrals { get; set;}
}
