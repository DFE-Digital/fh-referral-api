using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FamilyHubs.Referral.Data.Repository;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext
        (
            DbContextOptions<ApplicationDbContext> options,
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
        )
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Entities.ReferralService>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<Entities.Organisation>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<Entities.ReferralUserAccount>().Property(c => c.Id).ValueGeneratedNever();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<Recipient> Recipients => Set<Recipient>();
    public DbSet<Data.Entities.Referral> Referrals => Set<Data.Entities.Referral>(); 
    public DbSet<Entities.ReferralService> ReferralServices => Set<Entities.ReferralService>();
    public DbSet<ReferralStatus> ReferralStatuses => Set<ReferralStatus>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserAccountOrganisation> UserAccountOrganisations => Set<UserAccountOrganisation>();
    public DbSet<UserAccountRole> UserAccountRoles => Set<UserAccountRole>();
    public DbSet<UserAccountService> UserAccountServices => Set<UserAccountService>();
}
