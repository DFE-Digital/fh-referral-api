﻿using FamilyHubs.Referral.Data.Entities;
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
        if (Database.IsSqlServer())
        {
            modelBuilder.Entity<ReferralOrganisation>().ToTable("ReferralOrganisations", b => b.IsTemporal());
            modelBuilder.Entity<Entities.ReferralService>().ToTable("ReferralServices", b => b.IsTemporal());
            modelBuilder.Entity<Recipient>().ToTable("Recipients", b => b.IsTemporal());
            modelBuilder.Entity<Referrer>().ToTable("Referrers", b => b.IsTemporal());
            modelBuilder.Entity<ReferralStatus>().ToTable("ReferralStatuses", b => b.IsTemporal());
            modelBuilder.Entity<Entities.Referral>().ToTable("Referrals", b => b.IsTemporal());
        }

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    

    public DbSet<Data.Entities.Referral> Referrals => Set<Data.Entities.Referral>();
    public DbSet<Recipient> Recipients => Set<Recipient>();
    public DbSet<ReferralOrganisation> ReferralOrganisations => Set<ReferralOrganisation>();
    public DbSet<Entities.ReferralService> ReferralServices => Set<Entities.ReferralService>();
    public DbSet<ReferralStatus> ReferralStatuses => Set<ReferralStatus>();
    public DbSet<Referrer> Referrers => Set<Referrer>();
}
