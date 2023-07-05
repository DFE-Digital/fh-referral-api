using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Data.Repository;

public interface IApplicationDbContext
{
    DbSet<Data.Entities.Referral> Referrals { get; }
    DbSet<Status> Statuses { get; }
    DbSet<Recipient> Recipients { get; }
    DbSet<Organisation> Organisations { get; }
    DbSet<Entities.ReferralService> ReferralServices { get; }
    DbSet<UserAccount> UserAccounts { get; }
    DbSet<UserAccountOrganisation> UserAccountOrganisations { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserAccountRole> UserAccountRoles { get; }
    DbSet<UserAccountService> UserAccountServices { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
