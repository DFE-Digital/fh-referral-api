using FamilyHubs.Referral.Data.Entities;
using FamilyHubs.Referral.Data.Entities.Metrics;
using FamilyHubs.Referral.Data.Interceptors;
using FamilyHubs.SharedKernel.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using System.Reflection;

namespace FamilyHubs.Referral.Data.Repository;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly IEncryptionProvider _provider;

    public ApplicationDbContext
        (
            DbContextOptions<ApplicationDbContext> options,
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
            IKeyProvider keyProvider
        )
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;

        byte[]? byteencryptionKey;
        byte[]? byteencryptionIV;

        string? encryptionKey = keyProvider.GetDbEncryptionKey().Result;
        if (!string.IsNullOrEmpty(encryptionKey))
        {
            byteencryptionKey = ConvertStringToByteArray(encryptionKey);
        }
        else
        {
            throw new ArgumentException("EncryptionKey is missing");
        }
        string? encryptionIV = keyProvider.GetDbEncryptionIVKey().Result;
        if (!string.IsNullOrEmpty(encryptionIV))
        {
            byteencryptionIV = ConvertStringToByteArray(encryptionIV);
        }
        else
        {
            throw new ArgumentException("EncryptionIV is missing");
        }
        _provider = new AesProvider(byteencryptionKey, byteencryptionIV);

    }

    private byte[] ConvertStringToByteArray(string value)
    {
        List<byte> bytes = new List<byte>();
        string[] parts = value.Split(',');
        foreach (string part in parts)
        {
            if (byte.TryParse(part, out byte b))
            {
                bytes.Add(b);
            }
        }
        return bytes.ToArray();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Entities.ReferralService>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<Organisation>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<UserAccount>().Property(c => c.Id).ValueGeneratedNever();

        var organisationEntity = modelBuilder.Entity<Organisation>();
        organisationEntity.Property(x => x.CreatedBy).IsEncrypted();
        organisationEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var referralServiceEntity = modelBuilder.Entity<Data.Entities.ReferralService>();
        referralServiceEntity.Property(x => x.CreatedBy).IsEncrypted();
        referralServiceEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var roleEntity = modelBuilder.Entity<Role>();
        roleEntity.Property(x => x.CreatedBy).IsEncrypted();
        roleEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var statusEntity = modelBuilder.Entity<Status>();
        statusEntity.Property(x => x.CreatedBy).IsEncrypted();
        statusEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var recipientEntity = modelBuilder.Entity<Recipient>();
        recipientEntity.Property(x => x.Name).IsRequired().IsEncrypted();
        recipientEntity.Property(x => x.Email).IsEncrypted();
        recipientEntity.Property(x => x.Telephone).IsEncrypted();
        recipientEntity.Property(x => x.TextPhone).IsEncrypted();
        recipientEntity.Property(x => x.AddressLine1).IsEncrypted();
        recipientEntity.Property(x => x.AddressLine2).IsEncrypted();
        recipientEntity.Property(x => x.TownOrCity).IsEncrypted();
        recipientEntity.Property(x => x.County).IsEncrypted();
        recipientEntity.Property(x => x.PostCode).IsEncrypted();
        recipientEntity.Property(x => x.CreatedBy).IsEncrypted();
        recipientEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var referralEntity = modelBuilder.Entity<Entities.Referral>();
        referralEntity.Property(x => x.ReasonForSupport).IsEncrypted();
        referralEntity.Property(x => x.EngageWithFamily).IsEncrypted();
        referralEntity.Property(x => x.ReasonForDecliningSupport).IsEncrypted();
        referralEntity.Property(x => x.CreatedBy).IsEncrypted();
        referralEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var userEntity = modelBuilder.Entity<UserAccount>();
        userEntity.Property(x => x.EmailAddress).IsRequired().IsEncrypted();
        userEntity.Property(x => x.Name).IsEncrypted();
        userEntity.Property(x => x.PhoneNumber).IsEncrypted();
        userEntity.Property(x => x.Team).IsEncrypted();
        userEntity.Property(x => x.CreatedBy).IsEncrypted();
        userEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var userAccountOrganisationEntity = modelBuilder.Entity<UserAccountOrganisation>();
        userAccountOrganisationEntity.Property(x => x.CreatedBy).IsEncrypted();
        userAccountOrganisationEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var userAccountRoleEntity = modelBuilder.Entity<UserAccountRole>();
        userAccountRoleEntity.Property(x => x.CreatedBy).IsEncrypted();
        userAccountRoleEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        var userAccountRoleService = modelBuilder.Entity<UserAccountService>();
        userAccountRoleService.Property(x => x.CreatedBy).IsEncrypted();
        userAccountRoleService.Property(x => x.LastModifiedBy).IsEncrypted();

        var connectionRequestsSentMetricEntity = modelBuilder.Entity<ConnectionRequestsSentMetric>();
        connectionRequestsSentMetricEntity.Property(x => x.CreatedBy).IsEncrypted();
        connectionRequestsSentMetricEntity.Property(x => x.LastModifiedBy).IsEncrypted();

        modelBuilder.UseEncryption(this._provider);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<Recipient> Recipients => Set<Recipient>();
    public DbSet<Entities.Referral> Referrals => Set<Entities.Referral>();
    public DbSet<Entities.ReferralService> ReferralServices => Set<Entities.ReferralService>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserAccountOrganisation> UserAccountOrganisations => Set<UserAccountOrganisation>();
    public DbSet<UserAccountRole> UserAccountRoles => Set<UserAccountRole>();
    public DbSet<UserAccountService> UserAccountServices => Set<UserAccountService>();
    public DbSet<ConnectionRequestsSentMetric> ConnectionRequestsSentMetric => Set<ConnectionRequestsSentMetric>();
}