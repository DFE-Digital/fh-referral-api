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
        if (_context.Statuses.Any())
            return;

        IReadOnlyCollection<Entities.Status> statuses = StatusSeedData.SeedStatuses();

        _context.Statuses.AddRange(statuses);

        await _context.SaveChangesAsync();

        IReadOnlyCollection<Entities.Referral> referrals = ReferralSeedData.SeedReferral();

        foreach(Entities.Referral referral in referrals)
        {
            var status = _context.Statuses.FirstOrDefault(x => x.Id == referral.Status.Id); 
            if (status != null)
            {
                referral.Status = status; 
            }
        }

        _context.Referrals.AddRange(referrals);
        
        await _context.SaveChangesAsync();

    }
}
