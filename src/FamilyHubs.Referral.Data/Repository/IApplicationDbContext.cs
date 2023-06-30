using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Data.Repository;

public interface IApplicationDbContext
{
    DbSet<Data.Entities.Referral> Referrals { get; }
    DbSet<ReferralStatus> ReferralStatuses { get; }
    DbSet<Recipient> Recipients { get; }
    DbSet<Organisation> Organisations { get; }
    DbSet<Entities.ReferralService> ReferralServices { get; }
    DbSet<ReferralUserAccount> ReferralUserAccounts { get; }
    DbSet<UserAccount> UserAccounts { get; }
    DbSet<OrganisationUserAccount> OrganisationUserAccounts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
