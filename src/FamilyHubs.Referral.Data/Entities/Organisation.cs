namespace FamilyHubs.Referral.Data.Entities;

public class Organisation : EntityBase<long>
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    // An Organisation can have zero or many Services
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    // An Organisation can have zero or many Users
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    // An Organisation can have zero or many Teams
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    // An Organisation can have zero or many Roles
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
