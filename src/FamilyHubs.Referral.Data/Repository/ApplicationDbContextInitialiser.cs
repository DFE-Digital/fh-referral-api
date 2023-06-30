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
            await TrySeedAsync(_context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public static async Task TrySeedAsync(ApplicationDbContext context)
    {
        IReadOnlyCollection<ReferralStatus> statuses = ReferralSeedData.SeedStatuses();
        if (!context.ReferralStatuses.Any())
        {
            context.ReferralStatuses.AddRange(statuses);

            await context.SaveChangesAsync();
        }
        else
        {
            foreach (var seedStatus in statuses)
            {
                var dbStatus = context.ReferralStatuses.FirstOrDefault(x => x.Name == seedStatus.Name);
                if (!seedStatus.Equals(dbStatus))
                { 
                    if (dbStatus == null)
                    {
                        dbStatus = seedStatus;
                    }
                    else
                    {
                        dbStatus.Name = seedStatus.Name;
                        dbStatus.SortOrder = seedStatus.SortOrder;
                        dbStatus.SecondrySortOrder = seedStatus.SecondrySortOrder;
                    }
                    
                    context.ReferralStatuses.Update(dbStatus); 
                }
            }

            await context.SaveChangesAsync();
        }

        if (!context.ReferralOrganisations.Any())
        {
            context.ReferralOrganisations.AddRange(ReferralSeedData.SeedReferralOrganisations());

            await context.SaveChangesAsync();
        }

        if (context.Database.IsSqlite() && !context.Referrals.Any())
        {
            IReadOnlyCollection<Entities.Referral> referrals = ReferralSeedData.SeedReferral();

            foreach (Entities.Referral referral in referrals)
            {
                var status = context.ReferralStatuses.SingleOrDefault(x => x.Name == referral.Status.Name);
                if (status != null)
                {
                    referral.Status = status;
                }

                var referralOrganisation = context.ReferralOrganisations.SingleOrDefault(x => x.Id == referral.ReferralService.ReferralOrganisation.Id);
                if (referralOrganisation != null) 
                {
                    referralOrganisation.ReferralServiceId = 1;
                    referral.ReferralUserAccount.ReferralOrganisation = referralOrganisation;
                    referral.ReferralService.ReferralOrganisation = referralOrganisation;
                }
            }

            
            context.Referrals.AddRange(referrals);

            await context.SaveChangesAsync();
        }
    }
}
