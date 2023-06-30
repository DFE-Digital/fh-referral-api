﻿using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class ReferralConfiguration : IEntityTypeConfiguration<Data.Entities.Referral>
{
    public void Configure(EntityTypeBuilder<Data.Entities.Referral> builder)
    {
        builder.Navigation(e => e.Recipient).AutoInclude();
        builder.Navigation(e => e.ReferralUserAccount).AutoInclude();
        builder.Navigation(e => e.ReferralService).AutoInclude();
        builder.Navigation(e => e.Status).AutoInclude();

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();
    }
}
