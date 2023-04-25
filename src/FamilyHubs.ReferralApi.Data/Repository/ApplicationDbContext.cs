using Ardalis.GuardClauses;
using EncryptColumn.Core.Extension;
using EncryptColumn.Core.Interfaces;
using EncryptColumn.Core.Util;
using FamilyHubs.ReferralApi.Data.Entities;
using FamilyHubs.ReferralApi.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FamilyHubs.ReferralApi.Data.Repository;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
#if _USE_EVENT_DISPATCHER
    private readonly IDomainEventDispatcher _dispatcher;
#endif
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly IEncryptionProvider _provider;

    public ApplicationDbContext
        (
            DbContextOptions<ApplicationDbContext> options,
#if _USE_EVENT_DISPATCHER
            IDomainEventDispatcher dispatcher,
#endif
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
            IConfiguration configuration
        )
        : base(options)
    {
#if _USE_EVENT_DISPATCHER
        _dispatcher = dispatcher;
#endif
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        //DbKey should be overriden in build pipeline
        string dbKey = configuration.GetValue<string>("DbKey");
        if (string.IsNullOrEmpty(dbKey))
        {
            throw new NotFoundException(nameof(IConfiguration), "DbKey");
        }

        this._provider = new GenerateEncryptionProvider(dbKey);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.UseEncryption(this._provider);

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

#if _USE_EVENT_DISPATCHER

        // ignore events if no dispatcher provided
        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Select(e => e.Entity as EntityBase<string>)
            .Where(e => e?.DomainEvents != null && e.DomainEvents.Any())
            .ToArray();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        if (entitiesWithEvents != null && entitiesWithEvents.Any())
        {
            var entitiesWithEventsGuids = new List<EntityBase<Guid>>();
            foreach (var entityWithEvents in entitiesWithEvents)
            {
                var t = entityWithEvents?.ToString();
                entitiesWithEventsGuids.Add(Guid.Parse(t);
            }

            await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);
        }
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#endif
        return result;
    }

    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<ReferralStatus> ReferralStatuses => Set<ReferralStatus>();
}
