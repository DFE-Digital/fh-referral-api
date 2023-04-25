using FamilyHubs.ReferralApi.Data.Interceptors;
using FamilyHubs.ReferralApi.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyHubs.ReferralApi.Infrastructure.Tests;

public abstract class BaseEfRepositoryTestFixture
{
    protected ApplicationDbContext DbContext; // see https://social.msdn.microsoft.com/Forums/en-US/930f159f-dfa5-4aa8-9af6-aa6545da5cbd/what-is-the-c-protected-property-naming-convention?forum=csharpgeneral

    protected BaseEfRepositoryTestFixture()
    {
        var options = CreateNewContextOptions();
        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor();

        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("DbKey", "kljsdkkdlo4454GG00155sajuklmbkdl")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

#if _USE_EVENT_DISPATCHER
        DbContext = new ApplicationDbContext(options, new Mock<IDomainEventDispatcher>().Object, auditableEntitySaveChangesInterceptor, configuration);
#else
        DbContext = new ApplicationDbContext(options, auditableEntitySaveChangesInterceptor, configuration);
#endif
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
        builder.UseInMemoryDatabase("OrOpenReferralOrganisations")
               .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }
}