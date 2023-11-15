using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(Consts.NameMaxLength);

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(Consts.EmailMaxLength)
            .IsRequired();

        builder.Property(t => t.LastModifiedBy)
            .HasMaxLength(Consts.EmailMaxLength);
    }
}