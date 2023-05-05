﻿using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class ReferrerConfiguration : IEntityTypeConfiguration<Referrer>
{
    public void Configure(EntityTypeBuilder<Referrer> builder)
    {
        builder.Property(t => t.EmailAddress)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(255)
            .IsRequired();
    }
}

