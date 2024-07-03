using FamilyHubs.Referral.Data.Entities.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class ConnectionRequestsSentMetricConfiguration : IEntityTypeConfiguration<ConnectionRequestsSentMetric>
{
    public void Configure(EntityTypeBuilder<ConnectionRequestsSentMetric> builder)
    {
        builder.HasIndex(u => u.RequestCorrelationId)
            .IsUnique();

        builder.Property(t => t.RequestCorrelationId)
            .HasMaxLength(50);

        builder.Property(t => t.HttpResponseCode)
            .HasConversion<short?>()
            .HasColumnType("smallint");

        builder.Property(t => t.ConnectionRequestReferenceCode)
            .HasColumnType("nchar(6)");

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