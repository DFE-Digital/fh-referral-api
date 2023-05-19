using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Data.Repository;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync(bool isProduction, bool shouldRestDatabaseOnRestart)
    {
        try
        {
            if (!isProduction)
            {
                if (shouldRestDatabaseOnRestart)
                    await _context.Database.EnsureDeletedAsync();

                if (_context.Database.IsSqlServer())
                    await _context.Database.MigrateAsync();
                else
                    await _context.Database.EnsureCreatedAsync();

                await SeedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (!_context.ReferralStatuses.Any())
        {
            IReadOnlyCollection<ReferralStatus> statuses = ReferralSeedData.SeedStatuses();

            _context.ReferralStatuses.AddRange(statuses);

            await _context.SaveChangesAsync();
        }

        if (_context.Database.IsSqlite() && !_context.Referrals.Any())
        {
            IReadOnlyCollection<Entities.Referral> referrals = ReferralSeedData.SeedReferral();

            foreach (Entities.Referral referral in referrals)
            {
                var status = _context.ReferralStatuses.SingleOrDefault(x => x.Name == referral.Status.Name);
                if (status != null)
                {
                    referral.Status = status;
                }
            }

            _context.Referrals.AddRange(referrals);

            await _context.SaveChangesAsync();
        }
    }
}
