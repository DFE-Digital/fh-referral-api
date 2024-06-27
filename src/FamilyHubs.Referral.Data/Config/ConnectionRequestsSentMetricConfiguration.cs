using FamilyHubs.Referral.Data.Entities.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class ConnectionRequestsSentMetricConfiguration : IEntityTypeConfiguration<ConnectionRequestsSentMetric>
{
    public void Configure(EntityTypeBuilder<ConnectionRequestsSentMetric> builder)
    {
        builder.Property(t => t.ConnectionRequestReference)
            .HasMaxLength(10);

        //todo: better to have Created as non-nullable and don't explicitly set it as required?
        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(MaxLength.Email)
            .IsRequired();

        builder.Property(t => t.LastModifiedBy)
            .HasMaxLength(MaxLength.Email);
    }
}