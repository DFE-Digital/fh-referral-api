using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class ReferralConfiguration : IEntityTypeConfiguration<Entities.Referral>
{
    public void Configure(EntityTypeBuilder<Entities.Referral> builder)
    {
        builder.Navigation(e => e.Recipient).AutoInclude();
        builder.Navigation(e => e.UserAccount).AutoInclude();
        builder.Navigation(e => e.ReferralService).AutoInclude();
        builder.Navigation(e => e.Status).AutoInclude();

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(MaxLength.Email)
            .IsRequired();

        builder.Property(t => t.LastModifiedBy)
            .HasMaxLength(MaxLength.Email);
    }
}
