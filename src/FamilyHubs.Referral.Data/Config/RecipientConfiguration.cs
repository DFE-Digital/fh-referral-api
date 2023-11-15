﻿using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class RecipientConfiguration : IEntityTypeConfiguration<Recipient>
{
    public void Configure(EntityTypeBuilder<Recipient> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(Consts.NameMaxLength)
            .IsRequired();

        builder.Property(t => t.Email)
            .HasMaxLength(Consts.EmailMaxLength);

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(Consts.EmailMaxLength)
            .IsRequired();

        builder.Property(t => t.LastModifiedBy)
            .HasMaxLength(Consts.EmailMaxLength);
    }
}
