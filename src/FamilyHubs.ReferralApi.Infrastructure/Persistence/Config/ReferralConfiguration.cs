using FamilyHubs.ReferralApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.ReferralApi.Infrastructure.Persistence.Config;

public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.Navigation(e => e.Recipient).AutoInclude();
        builder.Navigation(e => e.Referrer).AutoInclude();
        builder.Navigation(e => e.ReferralService).AutoInclude();
        builder.Navigation(e => e.Status).AutoInclude();

        

        //builder.Property(t => t.Recipient)
        //    .IsRequired();
        //builder.Property(t => t.Referrer)
        //    .IsRequired();
        //builder.Property(t => t.ReferralService)
        //    .IsRequired();
        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();
    }
}
