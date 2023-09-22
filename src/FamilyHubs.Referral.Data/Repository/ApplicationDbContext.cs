using FamilyHubs.Referral.Data.Entities;
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
        modelBuilder.Entity<Entities.Organisation>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<Entities.UserAccount>().Property(c => c.Id).ValueGeneratedNever();

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

        var referralEntity = modelBuilder.Entity<Data.Entities.Referral>();
        referralEntity.Property(x => x.ReasonForSupport).IsEncrypted();
        referralEntity.Property(x => x.EngageWithFamily).IsEncrypted();
        referralEntity.Property(x => x.ReasonForDecliningSupport).IsEncrypted();

        var userEntity = modelBuilder.Entity<UserAccount>();
        userEntity.Property(x => x.EmailAddress).IsRequired().IsEncrypted();
        userEntity.Property(x => x.Name).IsEncrypted();
        userEntity.Property(x => x.PhoneNumber).IsEncrypted();
        userEntity.Property(x => x.Team).IsEncrypted();

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
    public DbSet<Data.Entities.Referral> Referrals => Set<Data.Entities.Referral>(); 
    public DbSet<Entities.ReferralService> ReferralServices => Set<Entities.ReferralService>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserAccountOrganisation> UserAccountOrganisations => Set<UserAccountOrganisation>();
    public DbSet<UserAccountRole> UserAccountRoles => Set<UserAccountRole>();
    public DbSet<UserAccountService> UserAccountServices => Set<UserAccountService>();
}
