using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.Referral.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyHubs.Referral.UnitTests;

public class BaseCreateDbUnitTest
{
    protected BaseCreateDbUnitTest()
    {
    }
    protected static ApplicationDbContext GetApplicationDbContext()
    {
        var options = CreateNewContextOptions();
        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor();

#if _USE_EVENT_DISPATCHER
        var mockApplicationDbContext = new ApplicationDbContext(options, new Mock<IDomainEventDispatcher>().Object, auditableEntitySaveChangesInterceptor, configuration);
#else
        var mockApplicationDbContext = new ApplicationDbContext(options,  auditableEntitySaveChangesInterceptor);
#endif
        return mockApplicationDbContext;
    }

    protected static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh
        // InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an
        // InMemory database and the new service provider.
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase("ReferralDb")
               .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    protected static async Task CreateReferrals(ApplicationDbContext context)
    {
        if (!context.ReferralStatuses.Any())
        {
            IReadOnlyCollection<ReferralStatus> statuses = ReferralSeedData.SeedStatuses();

            context.ReferralStatuses.AddRange(statuses);

            await context.SaveChangesAsync();
        }

        if (!context.ReferralServices.Any())
        {
            var referralService = new Data.Entities.ReferralService
            {
                Id = 1,
                Name = "Test Service",
                Description = "Test Service Description",
                ReferralOrganisation = new ReferralOrganisation
                {
                    Id = 1,
                    ReferralServiceId = 1,
                    Name = "Test Organisation",
                    Description = "Test Organisation Description",
                }
            };

            context.ReferralServices.Add(referralService);
            await context.SaveChangesAsync();
        }


        IReadOnlyCollection<Data.Entities.Referral> referrals = ReferralSeedData.SeedReferral(true);

        foreach (Data.Entities.Referral referral in referrals)
        {
            var referrer = context.Referrers.SingleOrDefault(x => x.Id == referral.Referrer.Id);
            if (referrer != null)
            {
                referral.Referrer = referrer;
            }

            var status = context.ReferralStatuses.SingleOrDefault(x => x.Name == referral.Status.Name);
            if (status != null)
            {
                referral.Status = status;
            }

            var service = context.ReferralServices.SingleOrDefault(x => x.Id == referral.ReferralService.Id);
            if (service != null)
            {
                referral.ReferralService = service;
            }

            context.Referrals.Add(referral);

            await context.SaveChangesAsync();
        }        
    }
}
