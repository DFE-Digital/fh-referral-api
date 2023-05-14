namespace FamilyHubs.Referral.Data.Entities;


public class Role : EntityBase<long>
{
    public required string Name { get; set; }

    // A Role can be associated with zero or many Organisations
    public virtual ICollection<Organisation>? Organisations { get; set; }
}