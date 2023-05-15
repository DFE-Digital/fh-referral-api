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

        IReadOnlyCollection<Data.Entities.Referral> referrals = ReferralSeedData.SeedReferral();

        TestDbContext.Referrals.AddRange(referrals);

        TestDbContext.SaveChangesAsync().GetAwaiter().GetResult(); 
    }

    protected async Task<ReferralDto> CreateReferral(ReferralDto? newReferral = null)
    {
        var referral = Mapper.Map<Data.Entities.Referral>(newReferral ?? TestDataProvider.GetReferralDto());

        Team? team = TestDbContext.Teams.SingleOrDefault(x => x.Name == TestDataProvider.GetReferralDto().TeamDto.Name);
        if (team == null)
        {
            var organisation = Mapper.Map<Organisation>(TestDataProvider.GetReferralDto().ServiceDto.OrganisationDto);
            Organisation? dborganisation = TestDbContext.Organisations.SingleOrDefault(x => x.Id == organisation.Id);
            if (dborganisation != null)
            {
                organisation = dborganisation;
            }
            team = new Team
            {
                Organisation = organisation,
                Name = TestDataProvider.GetReferralDto().TeamDto.Name,
            };

            TestDbContext.Teams.Add(team);
            TestDbContext.SaveChanges();

        }


        var status = TestDbContext.Statuses.SingleOrDefault(x => x.Id == referral.Status.Id);
        if (status != null)
        {
            referral.Status = status;
        }

        var existingteam = TestDbContext.Teams.SingleOrDefault(x => x.Id == referral.Team.Id);
        if (existingteam != null) 
        {
            referral.Team = existingteam;
        }

        var existingOrganisation = TestDbContext.Organisations.SingleOrDefault(x => x.Id == referral.Service.Organisation.Id);
        if (existingOrganisation != null)
        {
            referral.Service.Organisation = existingOrganisation;
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
