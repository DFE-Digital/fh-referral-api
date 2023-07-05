using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyHubs.Referral.Data.Entities;

public class UserAccount : EntityBase<long>
{
    public required string EmailAddress { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Team { get; set; }
    public virtual IList<UserAccountRole>? UserAccountRoles { get; set; }
    public virtual IList<UserAccountService>? ServiceUserAccounts { get; set;}
    public virtual IList<UserAccountOrganisation>? OrganisationUserAccounts { get; set; }
}
