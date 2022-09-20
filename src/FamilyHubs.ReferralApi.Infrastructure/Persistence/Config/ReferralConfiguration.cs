using FamilyHubs.ReferralApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.ReferralApi.Infrastructure.Persistence.Config;

public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.Property(t => t.FullName)
            .IsRequired();
        builder.Property(t => t.ServiceId)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(t => t.ServiceName)
            .IsRequired();
        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();
    }
}
