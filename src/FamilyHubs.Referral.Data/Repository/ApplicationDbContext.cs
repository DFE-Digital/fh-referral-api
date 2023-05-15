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

        // Set the primary key of the Referrals table to be the composite key of the dimension table foreign keys
        //modelBuilder.Entity<Entities.Referral>()
        //    .HasKey(r => new
        //    {
        //        ServiceId = r.Service.Id,
        //        //StatusId = r.Status.Id,
        //        //RecipientId = r.Recipient.Id,
        //        //TeamId = r.Team.Id,
        //        //UserId = r.Referrer.Id
        //    })
        //    .HasName("PK_ServiceId_StatusId_RecipientId_TeamId_ReferrerId");

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
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Entities.Service> Services => Set<Entities.Service>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<User> Users => Set<User>();

    
}
