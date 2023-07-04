using FamilyHubs.Referral.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class UserAccount : EntityBase<long>
{
    public required string EmailAddress { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public string? Team { get; set; }
    public virtual IList<OrganisationUserAccount>? OrganisationUserAccounts { get; set;}
}
