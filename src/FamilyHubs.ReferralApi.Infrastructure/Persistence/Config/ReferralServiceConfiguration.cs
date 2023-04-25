using FamilyHubs.ReferralApi.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHubs.ReferralApi.Infrastructure.Persistence.Config;


public class ReferralServiceConfiguration : IEntityTypeConfiguration<ReferralService>
{
    public void Configure(EntityTypeBuilder<ReferralService> builder)
    {
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

