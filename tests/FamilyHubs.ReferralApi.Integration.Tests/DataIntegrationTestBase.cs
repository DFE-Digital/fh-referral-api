using AutoMapper;
using AutoMapper.EquivalencyExpression;
using FamilyHubs.Referral.Core;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.Referral.Data.Repository;
using FamilyHubs.ReferralService.Shared.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace FamilyHubs.Referral.Integration.Tests;

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

        return new ServiceCollection()
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

        TestDbContext.ReferralStatuses.AddRange(ReferralSeedData.SeedStatuses());

        TestDbContext.SaveChangesAsync().GetAwaiter().GetResult();

        ApplicationDbContextInitialiser.TrySeedAsync(TestDbContext).GetAwaiter().GetResult();
    }

    protected async Task<ReferralDto> CreateReferral(ReferralDto? newReferral = null)
    {

        var referral = Mapper.Map<Data.Entities.Referral>(newReferral ?? TestDataProvider.GetReferralDto());

        var status = TestDbContext.ReferralStatuses.SingleOrDefault(x => x.Id == referral.Status.Id);
        if (status != null)
        {
            referral.Status = status;
        }

        var referralOrganisation = TestDbContext.ReferralOrganisations.SingleOrDefault(x => x.Id == referral.ReferralService.ReferralOrganisation.Id);
        if(referralOrganisation == null) 
        {
            TestDbContext.ReferralOrganisations.Add(referral.ReferralService.ReferralOrganisation);
            await TestDbContext.SaveChangesAsync();
            referralOrganisation = TestDbContext.ReferralOrganisations.SingleOrDefault(x => x.Id == referral.ReferralService.ReferralOrganisation.Id);
        }
        
        
        if (referralOrganisation != null)
        {
            referralOrganisation.ReferralServiceId = 1;
            referral.ReferralUserAccount.ReferralOrganisation = referralOrganisation;
            referral.ReferralService.ReferralOrganisation = referralOrganisation;
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
