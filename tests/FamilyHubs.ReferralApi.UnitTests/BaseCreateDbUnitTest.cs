using FamilyHubs.ReferralApi.Core;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Interceptors;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FamilyHubs.ReferralApi.UnitTests;

public class BaseCreateDbUnitTest
{
    protected static ApplicationDbContext GetApplicationDbContext()
    {
        var options = CreateNewContextOptions();
        var mockDateTime = new Mock<IDateTime>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor(mockCurrentUserService.Object, mockDateTime.Object);

        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("DbKey", "kljsdkkdlo4454GG00155sajuklmbkdl")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

#if _USE_EVENT_DISPATCHER
        var mockApplicationDbContext = new ApplicationDbContext(options, new Mock<IDomainEventDispatcher>().Object, auditableEntitySaveChangesInterceptor, configuration);
#else
        var mockApplicationDbContext = new ApplicationDbContext(options,  auditableEntitySaveChangesInterceptor, configuration);
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
}
