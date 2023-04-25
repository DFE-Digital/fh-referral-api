using FamilyHubs.ReferralApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.ReferralApi.Data.Config;

public class ReferralStatusConfiguration : IEntityTypeConfiguration<ReferralStatus>
{
    public void Configure(EntityTypeBuilder<ReferralStatus> builder)
    {
        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.ReferralId)
            .IsRequired();

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();
    }
}