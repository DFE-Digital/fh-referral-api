using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.SharedKernel.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Diagnostics;
using System.Security.Claims;

namespace FamilyHubs.Referral.UnitTests;

public class BaseCreateDbUnitTest
{
    protected string ExpectedRequestCorrelationId { get; set; }

    protected BaseCreateDbUnitTest()
    {
        var activity = new Activity("TestActivity");
        activity.SetParentId(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom());
        activity.Start();
        Activity.Current = activity;
        ExpectedRequestCorrelationId = Activity.Current!.TraceId.ToString();
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

        var inMemorySettings = new Dictionary<string, string?> {
            {"Crypto:UseKeyVault", "False"},
            {"Crypto:DbEncryptionKey", "188,7,221,249,250,101,147,86,47,246,21,252,145,56,161,150,195,184,64,43,55,0,196,200,98,220,95,186,225,8,224,75"},
            {"Crypto:DbEncryptionIVKey", "34,26,215,81,137,34,109,107,236,206,253,62,115,38,65,112"},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor(mockIHttpContextAccessor.Object);
        var keyProvider = new KeyProvider(configuration);

#if _USE_EVENT_DISPATCHER
        var mockApplicationDbContext = new ApplicationDbContext(options, new Mock<IDomainEventDispatcher>().Object, auditableEntitySaveChangesInterceptor, configuration);
#else
        var mockApplicationDbContext = new ApplicationDbContext(options,  auditableEntitySaveChangesInterceptor, keyProvider);
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
