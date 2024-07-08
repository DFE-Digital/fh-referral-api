using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Entities.Metrics;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Data.Repository;

public interface IApplicationDbContext
{
    DbSet<Entities.Referral> Referrals { get; }
    DbSet<Status> Statuses { get; }
    DbSet<Recipient> Recipients { get; }
    DbSet<Organisation> Organisations { get; }
    DbSet<Entities.ReferralService> ReferralServices { get; }
    DbSet<UserAccount> UserAccounts { get; }
    DbSet<UserAccountOrganisation> UserAccountOrganisations { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserAccountRole> UserAccountRoles { get; }
    DbSet<UserAccountService> UserAccountServices { get; }
    DbSet<ConnectionRequestsSentMetric> ConnectionRequestsSentMetric { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}