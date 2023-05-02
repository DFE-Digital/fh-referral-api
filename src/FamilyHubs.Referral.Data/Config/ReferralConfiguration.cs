using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class ReferralConfiguration : IEntityTypeConfiguration<Data.Entities.Referral>
{
    public void Configure(EntityTypeBuilder<Data.Entities.Referral> builder)
    {
        builder.Navigation(e => e.Recipient).AutoInclude();
        builder.Navigation(e => e.Referrer).AutoInclude();
        builder.Navigation(e => e.ReferralService).AutoInclude();
        builder.Navigation(e => e.Status).AutoInclude();

        builder.HasMany(s => s.Status)
            .WithOne()
            .HasForeignKey(lc => lc.ReferralId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Recipient)
            .WithOne()
            .HasForeignKey<Recipient>(lc => lc.ReferralId)
            .IsRequired();

        builder.HasOne(s => s.Referrer)
            .WithOne()
            .HasForeignKey<Referrer>(lc => lc.ReferralId)
            .IsRequired();

        builder.HasOne(s => s.ReferralService)
            .WithOne()
            .HasForeignKey<Entities.ReferralService>(lc => lc.ReferralId)
            .IsRequired();

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();
    }
}
