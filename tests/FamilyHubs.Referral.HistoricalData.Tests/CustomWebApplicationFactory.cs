using FamilyHubs.Referral.Api;
using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.Referral.Data.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FamilyHubs.Referral.HistoricalData.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _referralConnection;
    public CustomWebApplicationFactory()
    {
        _referralConnection = $"Data Source=BRSM0011\\SQLEXPRESS;Initial Catalog=FamilyHubs.Referral.{Random.Shared.Next().ToString()}.Database;Integrated Security=True;MultipleActiveResultSets=True;Pooling=False;Connect Timeout=30;TrustServerCertificate=True";
        //_referralConnection = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FamilyHubs.Referral.Database;Integrated Security=True;MultipleActiveResultSets=True;Pooling=False;Connect Timeout=30;"
    }

    public IServiceProvider ServiceProvider { get => Services; }

    /// <summary>
    /// Overriding CreateHost to avoid creating a separate ServiceProvider per this thread:
    /// https://github.com/dotnet-architecture/eShopOnWeb/issues/465
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = builder.Build();
        host.Start();

        // Get service provider.
        var serviceProvider = host.Services;

        // Create a scope to obtain a reference to the database
        // context (AppDbContext).
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            // Ensure the database is created.
            db.Database.EnsureCreated();
        }

        return host;
    }

    public async Task ModifyQuartzTrigger()
    {
        using var scope = Services.CreateScope();

        var scopedServices = scope.ServiceProvider;
        ISchedulerFactory schedulerFactory = scopedServices.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();
        var oldTriggerKey = new TriggerKey("DeleteHistoricalDataJob-trigger");

        //Create a new trigger with a new schedule
        var newTrigger = TriggerBuilder.Create()
            .WithIdentity("DeleteHistoricalDataJob-trigger")
            .WithCronSchedule("0/5 * * * * ?") // run every day at 1am
            .Build();

        //Reschedule the job with the new trigger
        await scheduler.RescheduleJob(oldTriggerKey, newTrigger);

    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dateTimeService = services.Where(s => s.ServiceType.FullName?.Contains("IDateTime") == true).ToList();    

            if (dateTimeService != null && dateTimeService.Any())
            {
                dateTimeService.ForEach(s => services.Remove(s));

                services.AddTransient<IDateTime, HistoricalDateTimeService>();
            }

            var efCoreServices = services.Where(s =>
                s.ServiceType.FullName?.Contains("EntityFrameworkCore") == true).ToList();

            efCoreServices.ForEach(s => services.Remove(s));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_referralConnection, mg =>
                    mg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString()));
            });

        });

        builder.UseEnvironment("Development");
    }

    public void SetupTestDatabaseAndSeedData()
    {
        using var scope = Services.CreateScope();

        var scopedServices = scope.ServiceProvider;
        var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

        try
        {
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();


            IReadOnlyCollection<ReferralStatus> statuses = ReferralSeedData.SeedStatuses();

            if (!context.ReferralStatuses.Any())
            {
                context.ReferralStatuses.AddRange(statuses);
                context.SaveChanges();
            }

            if (!context.Referrals.Any())
            {
                IReadOnlyCollection<Data.Entities.Referral> referrals = ReferralSeedData.SeedReferral(true);

                foreach (Data.Entities.Referral referral in referrals)
                {
                    var status = context.ReferralStatuses.SingleOrDefault(x => x.Name == referral.Status.Name);
                    if (status != null)
                    {
                        referral.Status = status;
                    }

                    var referralService = context.ReferralServices.SingleOrDefault(x => x.Id == referral.ReferralService.Id);
                    if (referralService != null)
                    {
                        referral.ReferralService = referralService;
                    }

                    var organisation = context.ReferralOrganisations.SingleOrDefault(x => x.Id == referral.ReferralService.ReferralOrganisation.Id);
                    if (organisation != null)
                    {
                        referral.ReferralService.ReferralOrganisation = organisation;
                    }

                    context.Referrals.Add(referral);
                    context.SaveChanges();

                }

                
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {exceptionMessage}", ex.Message);
        }
    }
}

