﻿// <auto-generated />
using System;
using FamilyHubs.Referral.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FamilyHubs.Referral.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230509123520_AdditionalProfessionalProperties")]
    partial class AdditionalProfessionalProperties
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Recipient", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("AddressLine1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PostCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ReferralId")
                        .HasColumnType("bigint");

                    b.Property<string>("Telephone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TownOrCity")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ReferralId")
                        .IsUnique();

                    b.ToTable("Recipients");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Referral", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EngageWithFamily")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReasonForDecliningSupport")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReasonForSupport")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Referrals");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralOrganisation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<long>("ReferralServiceId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ReferralServiceId")
                        .IsUnique();

                    b.ToTable("ReferralOrganisations");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralService", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<long>("ReferralId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ReferralId")
                        .IsUnique();

                    b.ToTable("ReferralServices");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ReferralId")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("ReferralId");

                    b.ToTable("ReferralStatuses");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Referrer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ReferralId")
                        .HasColumnType("bigint");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Team")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ReferralId")
                        .IsUnique();

                    b.ToTable("Referrers");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Recipient", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.Referral", null)
                        .WithOne("Recipient")
                        .HasForeignKey("FamilyHubs.Referral.Data.Entities.Recipient", "ReferralId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralOrganisation", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.ReferralService", null)
                        .WithOne("ReferralOrganisation")
                        .HasForeignKey("FamilyHubs.Referral.Data.Entities.ReferralOrganisation", "ReferralServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralService", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.Referral", null)
                        .WithOne("ReferralService")
                        .HasForeignKey("FamilyHubs.Referral.Data.Entities.ReferralService", "ReferralId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralStatus", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.Referral", null)
                        .WithMany("Status")
                        .HasForeignKey("ReferralId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Referrer", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.Referral", null)
                        .WithOne("Referrer")
                        .HasForeignKey("FamilyHubs.Referral.Data.Entities.Referrer", "ReferralId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Referral", b =>
                {
                    b.Navigation("Recipient")
                        .IsRequired();

                    b.Navigation("ReferralService")
                        .IsRequired();

                    b.Navigation("Referrer")
                        .IsRequired();

                    b.Navigation("Status");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralService", b =>
                {
                    b.Navigation("ReferralOrganisation")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
