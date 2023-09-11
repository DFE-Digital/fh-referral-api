using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.Referral.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace FamilyHubs.Referral.UnitTests;

public class BaseCreateDbUnitTest
{
    protected BaseCreateDbUnitTest()
    {
    }
    protected static ApplicationDbContext GetApplicationDbContext()
    {
        var options = CreateNewContextOptions();
        var mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "John Doe"),
            new Claim("OrganisationId", "1"),
            new Claim("AccountId", "2"),
            new Claim("AccountStatus", "Active"),
            new Claim("Name", "John Doe"),
            new Claim("ClaimsValidTillTime", "2023-09-11T12:00:00Z"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "john@example.com"),
            new Claim("PhoneNumber", "123456789")
        }, "test"));

        mockIHttpContextAccessor.Setup(h => h.HttpContext).Returns(context);

        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor(mockIHttpContextAccessor.Object);

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
        if (!context.Statuses.Any())
        {
            IReadOnlyCollection<Status> statuses = ReferralSeedData.SeedStatuses();

            context.Statuses.AddRange(statuses);

            await context.SaveChangesAsync();
        }

        if (!context.ReferralServices.Any())
        {
            var referralService = new Data.Entities.ReferralService
            {
                Id = 1,
                Name = "Test Service",
                Description = "Test Service Description",
                Organisation = new Organisation
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
            var referrer = context.UserAccounts.SingleOrDefault(x => x.Id == referral.UserAccount.Id);
            if (referrer != null)
            {
                referral.UserAccount = referrer;
            }

            var status = context.Statuses.SingleOrDefault(x => x.Name == referral.Status.Name);
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
