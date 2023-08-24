using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.SharedKernel.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyHubs.Referral.Data.Repository;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ICrypto _crypto;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, ICrypto crypto)
    {
        _logger = logger;
        _context = context;
        _crypto = crypto;
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
        IReadOnlyCollection<Status> statuses = ReferralSeedData.SeedStatuses();
        if (!_context.Statuses.Any())
        {
            _context.Statuses.AddRange(statuses);

            await _context.SaveChangesAsync();
        }
        else
        {
            foreach (var seedStatus in statuses)
            {
                var dbStatus = _context.Statuses.FirstOrDefault(x => x.Name == seedStatus.Name);
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
                    
                    _context.Statuses.Update(dbStatus); 
                }
            }

            await _context.SaveChangesAsync();
        }

        IReadOnlyCollection<Role> roles = ReferralSeedData.SeedRoles();
        if (!_context.Roles.Any())
        {
            _context.Roles.AddRange(roles);

            await _context.SaveChangesAsync();
        }
        else
        {
            foreach (var seedRole in roles)
            {
                var dbRole = _context.Roles.FirstOrDefault(x => x.Name == seedRole.Name);
                if (!seedRole.Equals(dbRole))
                {
                    if (dbRole == null)
                    {
                        dbRole = seedRole;
                    }
                    else
                    {
                        dbRole.Name = seedRole.Name;
                        dbRole.Description = seedRole.Description;
                        
                    }

                    _context.Roles.Update(dbRole);
                }
            }

            await _context.SaveChangesAsync();
        }
        

        if (_context.Database.IsSqlite() && !_context.Referrals.Any())
        {
            IReadOnlyCollection<Entities.Referral> referrals = ReferralSeedData.SeedReferral();

            foreach (Entities.Referral referral in referrals)
            {
                var status = _context.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
                if (status != null)
                {
                    referral.Status = status;
                }

                referral.ReasonForSupport = await _crypto.EncryptData(referral.ReasonForSupport);
                referral.EngageWithFamily = await _crypto.EncryptData(referral.EngageWithFamily);

                
                referral.Recipient.Name = !string.IsNullOrEmpty(referral.Recipient.Name) ? await _crypto.EncryptData(referral.Recipient.Name) : referral.Recipient.Name;
                referral.Recipient.Email = !string.IsNullOrEmpty(referral.Recipient.Email) ? await _crypto.EncryptData(referral.Recipient.Email) : referral.Recipient.Email;
                referral.Recipient.Telephone = !string.IsNullOrEmpty(referral.Recipient.Telephone) ? await _crypto.EncryptData(referral.Recipient.Telephone) : referral.Recipient.Telephone;
                referral.Recipient.TextPhone = !string.IsNullOrEmpty(referral.Recipient.TextPhone) ? await _crypto.EncryptData(referral.Recipient.TextPhone) : referral.Recipient.TextPhone;
                referral.Recipient.AddressLine1 = !string.IsNullOrEmpty(referral.Recipient.AddressLine1) ? await _crypto.EncryptData(referral.Recipient.AddressLine1) : referral.Recipient.AddressLine1;
                referral.Recipient.AddressLine2 = !string.IsNullOrEmpty(referral.Recipient.AddressLine2) ? await _crypto.EncryptData(referral.Recipient.AddressLine2) : referral.Recipient.AddressLine2;
                referral.Recipient.TownOrCity = !string.IsNullOrEmpty(referral.Recipient.TownOrCity) ? await _crypto.EncryptData(referral.Recipient.TownOrCity) : referral.Recipient.TownOrCity;
                referral.Recipient.County = !string.IsNullOrEmpty(referral.Recipient.County) ? await _crypto.EncryptData(referral.Recipient.County) : referral.Recipient.County;
                referral.Recipient.PostCode = !string.IsNullOrEmpty(referral.Recipient.PostCode) ? await _crypto.EncryptData(referral.Recipient.PostCode) : referral.Recipient.PostCode;


            }

            _context.Referrals.AddRange(referrals);

            await _context.SaveChangesAsync();
        }
    }
}
