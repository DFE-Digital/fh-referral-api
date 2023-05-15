namespace FamilyHubs.Referral.Data.Entities;


public class Team : EntityBase<long>
{
    public required string Name { get; set; }

    // A Team is associated with one Organisation
    public Organisation Organisation { get; set; }

    // A Team can have zero or many Users
    //public virtual required ICollection<User> Users { get; set; }

    // A Team can be associated with one or many Referrals
    //public virtual required ICollection<Referral> Referrals { get; set; }
}