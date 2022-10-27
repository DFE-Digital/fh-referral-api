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
    protected ApplicationDbContext GetApplicationDbContext()
    {
        var options = CreateNewContextOptions();
        var mockEventDispatcher = new Mock<IDomainEventDispatcher>();
        var mockDateTime = new Mock<IDateTime>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        var inMemorySettings = new Dictionary<string, string> {
            {"DbKey", "kljsdkkdlo4454GG00155sajuklmbkdl" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var mockConfig = new Mock<IConfiguration>();
        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor(mockCurrentUserService.Object, mockDateTime.Object);
        var mockApplicationDbContext = new ApplicationDbContext(options, mockEventDispatcher.Object, auditableEntitySaveChangesInterceptor, configuration);
       


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
