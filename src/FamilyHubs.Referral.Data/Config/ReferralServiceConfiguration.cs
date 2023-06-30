using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;


public class ReferralServiceConfiguration : IEntityTypeConfiguration<Entities.ReferralService>
{
    public void Configure(EntityTypeBuilder<Entities.ReferralService> builder)
    {
        builder.Navigation(e => e.ReferralOrganisation).AutoInclude();

        builder.HasOne(s => s.ReferralOrganisation)
           .WithOne()
           .HasForeignKey<ReferralOrganisation>(lc => lc.ReferralServiceId)
           .IsRequired(false);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();
    }
}

