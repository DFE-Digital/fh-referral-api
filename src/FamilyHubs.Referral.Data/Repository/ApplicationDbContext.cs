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
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Entities.ReferralService>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<Entities.ReferralOrganisation>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<Entities.ReferralUserAccount>().Property(c => c.Id).ValueGeneratedNever();

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
    public DbSet<ReferralUserAccount> Referrers => Set<ReferralUserAccount>();
}
