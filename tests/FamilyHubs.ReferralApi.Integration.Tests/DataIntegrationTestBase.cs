using AutoMapper;
using AutoMapper.EquivalencyExpression;
using FamilyHubs.ReferralApi.Core;
using FamilyHubs.ReferralApi.Data.Entities;
using FamilyHubs.ReferralApi.Data.Interceptors;
using FamilyHubs.ReferralApi.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace FamilyHubs.ReferralApi.Integration.Tests;

#pragma warning disable S3881
public abstract class DataIntegrationTestBase : IDisposable, IAsyncDisposable
{
    public IMapper Mapper { get; }
    public ApplicationDbContext TestDbContext { get; }
    public static NullLogger<T> GetLogger<T>() => new NullLogger<T>();

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

        var auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor();

        IEnumerable<KeyValuePair<string, string?>>? inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("DbKey", "kljsdkkdlo4454GG00155sajuklmbkdl")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        return new ServiceCollection()
            .AddSingleton(configuration)
            .AddEntityFrameworkSqlite()
            .AddDbContext<ApplicationDbContext>(dbContextOptionsBuilder =>
            {
                dbContextOptionsBuilder.UseSqlite(serviceDirectoryConnection, opt =>
                {
                    opt.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString());
                });
            })
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

        IReadOnlyCollection<Referral> referrals = ReferralSeedData.SeedReferral();

        TestDbContext.Referrals.AddRange(referrals);

        TestDbContext.SaveChangesAsync().GetAwaiter().GetResult(); 
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
