using AutoMapper;
using AutoMapper.EquivalencyExpression;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using FamilyHubs.SharedKernel.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FamilyHubs.Referral.Integration.Tests;

#pragma warning disable S3881
public abstract class DataIntegrationTestBase : IDisposable, IAsyncDisposable
{
    public IMapper Mapper { get; }
    public ApplicationDbContext TestDbContext { get; }

    protected DataIntegrationTestBase()
    {
        var serviceProvider = CreateNewReferralProvider();

        TestDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        Mapper = serviceProvider.GetRequiredService<IMapper>();

        InitialiseDatabase();
    }

    protected static ServiceProvider CreateNewReferralProvider()
    {
        var serviceDirectoryConnection = $"Data Source=sd-{Random.Shared.Next().ToString()}.db;Mode=ReadWriteCreate;Cache=Shared;Foreign Keys=True;Recursive Triggers=True;Default Timeout=30;Pooling=True";

        var mockIHttpContextAccessor = Mock.Of<IHttpContextAccessor>();
        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor(mockIHttpContextAccessor);

        var inMemorySettings = new Dictionary<string, string?> {
            {"Crypto:UseKeyVault", "False"},
            {"Crypto:DbEncryptionKey", "188,7,221,249,250,101,147,86,47,246,21,252,145,56,161,150,195,184,64,43,55,0,196,200,98,220,95,186,225,8,224,75"},
            {"Crypto:DbEncryptionIVKey", "34,26,215,81,137,34,109,107,236,206,253,62,115,38,65,112"},
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        IKeyProvider keyProvider = new KeyProvider(configuration);

        return new ServiceCollection()
            .AddEntityFrameworkSqlite()
            .AddDbContext<ApplicationDbContext>(dbContextOptionsBuilder =>
            {
                dbContextOptionsBuilder.UseSqlite(serviceDirectoryConnection, opt =>
                {
                    opt.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString());
                });
            })
            .AddSingleton(keyProvider)
            .AddSingleton(auditableEntitySaveChangesInterceptor)
            .AddAutoMapper((serviceProvider, cfg) =>
            {
                var auditProperties = new[] { "CreatedBy", "Created", "LastModified", "LastModified" };
                cfg.AddProfile<AutoMappingProfiles>();
                cfg.AddCollectionMappers();
                cfg.UseEntityFrameworkCoreModel<ApplicationDbContext>(serviceProvider);
                cfg.ShouldMapProperty = pi => !auditProperties.Contains(pi.Name);
            }, typeof(AutoMappingProfiles))
            .BuildServiceProvider();
    }

    private void InitialiseDatabase()
    {
        TestDbContext.Database.EnsureDeleted();
        TestDbContext.Database.EnsureCreated();

        TestDbContext.Statuses.AddRange(ReferralSeedData.SeedStatuses());

        TestDbContext.SaveChangesAsync().GetAwaiter().GetResult();

        IReadOnlyCollection<Data.Entities.Referral> referrals = ReferralSeedData.SeedReferral();

        foreach (Data.Entities.Referral referral in referrals)
        {
            var status = TestDbContext.Statuses.SingleOrDefault(x => x.Id == referral.Status.Id);
            if (status != null)
            {
                referral.Status = status;
            }
        }

        TestDbContext.Referrals.AddRange(referrals);

        TestDbContext.SaveChangesAsync().GetAwaiter().GetResult(); 
    }

    protected async Task<ReferralDto> CreateReferral(ReferralDto? newReferral = null)
    {

        var referral = Mapper.Map<Data.Entities.Referral>(newReferral ?? TestDataProvider.GetReferralDto());

        var status = TestDbContext.Statuses.SingleOrDefault(x => x.Id == referral.Status.Id);
        if (status != null)
        {
            referral.Status = status;
        }

        TestDbContext.Referrals.Add(referral);

        await TestDbContext.SaveChangesAsync();

        return Mapper.Map(referral, newReferral ?? TestDataProvider.GetReferralDto());
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await TestDbContext.Database.EnsureDeletedAsync();
    }

}
