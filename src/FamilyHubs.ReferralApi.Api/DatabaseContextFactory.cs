using FamilyHubs.ReferralApi.Infrastructure.Persistence.Interceptors;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using FamilyHubs.ReferralApi.Infrastructure.Service;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace FamilyHubs.ReferralApi.Api;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        string useDbType = configuration.GetValue<string>("UseDbType");

        switch (useDbType)
        {
            default:
                builder.UseInMemoryDatabase("FH-LAHubDb");
                break;

            case "UseSqlServerDatabase":
                {
                    var connectionString = configuration.GetConnectionString("ReferralConnection");
                    if (connectionString != null)
                        builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("FamilyHubs.ReferralApi.Api"));

                }
                break;

            case "UsePostgresDatabase":
                {
                    var connectionString = configuration.GetConnectionString("ReferralConnection");
                    if (connectionString != null)
                        builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("FamilyHubs.ReferralApi.Api"));

                }
                break;
        }

        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor = new(new CurrentUserService(new HttpContextAccessor()), new DateTimeService());

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#if _USE_EVENT_DISPATCHER
    return new ApplicationDbContext(builder.Options, null, auditableEntitySaveChangesInterceptor, configuration);
#else
        return new ApplicationDbContext(builder.Options, auditableEntitySaveChangesInterceptor, configuration);
#endif
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
}