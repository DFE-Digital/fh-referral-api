using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.Referral.Data.Repository;

public interface IApplicationDbContext
{
    DbSet<Data.Entities.Referral> Referrals { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
