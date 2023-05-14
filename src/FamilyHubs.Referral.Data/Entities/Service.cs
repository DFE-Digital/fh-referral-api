namespace FamilyHubs.Referral.Data.Entities;

public class Service : EntityBase<long>
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    // A Service is provided by one Organisation
    public virtual required Organisation Organisation { get; set; }

    // A Service is associated with one or many Referrals
    public virtual required ICollection<Referral> Referrals { get; set; }
}
