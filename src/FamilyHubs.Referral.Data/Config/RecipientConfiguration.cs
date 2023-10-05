using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class RecipientConfiguration : IEntityTypeConfiguration<Recipient>
{
    public void Configure(EntityTypeBuilder<Recipient> builder)
    {
        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.AddressLine1)
            .HasMaxLength(100);

        builder.Property(t => t.AddressLine2)
            .HasMaxLength(100);

        builder.Property(t => t.TownOrCity)
            .HasMaxLength(60);

        builder.Property(t => t.County)
            .HasMaxLength(60);

        builder.Property(t => t.PostCode)
            .HasMaxLength(15);

        builder.Property(t => t.Telephone)
            .HasMaxLength(20);

        builder.Property(t => t.TextPhone)
            .HasMaxLength(20);

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(320)
            .IsRequired();
        builder.Property(t => t.LastModifiedBy)
            .HasMaxLength(320);
    }
}
