using FamilyHubs.ReferralApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Data.Repository;

public interface IApplicationDbContext
{
    DbSet<Referral> Referrals { get; }
    DbSet<ReferralStatus> ReferralStatuses { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
