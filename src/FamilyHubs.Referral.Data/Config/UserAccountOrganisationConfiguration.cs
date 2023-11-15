using FamilyHubs.Referral.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHubs.Referral.Data.Config;

public class UserAccountOrganisationConfiguration : IEntityTypeConfiguration<UserAccountOrganisation>
{
    public void Configure(EntityTypeBuilder<UserAccountOrganisation> builder)
    {
        builder.Property(t => t.Created)
            .IsRequired();
        builder.Property(t => t.CreatedBy)
            .HasMaxLength(Consts.AuditByMaxLength)
            .IsRequired();

        builder.Property(t => t.LastModifiedBy)
            .HasMaxLength(Consts.AuditByMaxLength);
    }
}