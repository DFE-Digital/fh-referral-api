using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using FamilyHubs.ReferralApi.Core.Entities;
using FamilyHubs.ReferralApi.Core.Infrastructure;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Interceptors;
using FamilyHubs.SharedKernel;
using FamilyHubs.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly IEncryptionProvider _encryptionProvider;
    private readonly IConfiguration _configuration;

    public ApplicationDbContext
        (
            DbContextOptions<ApplicationDbContext> options,
            IDomainEventDispatcher dispatcher,
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
            IConfiguration configuration
        )
        : base(options)
    {
        _dispatcher = dispatcher;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _configuration = configuration;
        _encryptionProvider = new GenerateEncryptionProvider(_configuration["DbKey"]);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseEncryption(_encryptionProvider);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // ignore events if no dispatcher provided
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Select(e => e.Entity as EntityBase<string>)
            .Where(e => e?.DomainEvents != null && e.DomainEvents.Any())
            .ToArray();

        if (entitiesWithEvents != null && entitiesWithEvents.Any())
        {
            //var entitiesWithEventsGuids = new List<EntityBase<Guid>>();
            //foreach (var entityWithEvents in entitiesWithEvents)
            //{
            //    var t = entityWithEvents?.ToString();
            //    entitiesWithEventsGuids.Add(Guid.Parse(t);
            //}

            //await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);
        }

        return result;
    }

    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<ReferralStatus> ReferralStatuses => Set<ReferralStatus>();
}
