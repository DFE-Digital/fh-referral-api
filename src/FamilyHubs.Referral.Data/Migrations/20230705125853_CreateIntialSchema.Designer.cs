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
    [Migration("20230705125853_CreateIntialSchema")]
    partial class CreateIntialSchema
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Organisation", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

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

                    b.Property<long?>("ReferralServiceId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ReferralServiceId")
                        .IsUnique()
                        .HasFilter("[ReferralServiceId] IS NOT NULL");

                    b.ToTable("Organisations");
                });

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

                    b.Property<string>("County")
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

                    b.Property<string>("Telephone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TownOrCity")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

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

                    b.Property<long>("RecipientId")
                        .HasColumnType("bigint");

                    b.Property<long>("ReferralServiceId")
                        .HasColumnType("bigint");

                    b.Property<string>("ReferrerTelephone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("StatusId")
                        .HasColumnType("tinyint");

                    b.Property<long>("UserAccountId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("ReferralServiceId");

                    b.HasIndex("StatusId");

                    b.HasIndex("UserAccountId");

                    b.ToTable("Referrals");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralService", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

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

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ReferralServices");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralStatus", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint");

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<byte>("SecondrySortOrder")
                        .HasColumnType("tinyint");

                    b.Property<byte>("SortOrder")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.ToTable("ReferralStatuses");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Role", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint");

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

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccount", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Team")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccountOrganisation", b =>
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

                    b.Property<long>("OrganisationId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserAccountId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("UserAccountId");

                    b.ToTable("UserAccountOrganisations");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccountRole", b =>
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

                    b.Property<byte>("RoleId")
                        .HasColumnType("tinyint");

                    b.Property<long>("UserAccountId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserAccountId");

                    b.ToTable("UserAccountRoles");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccountService", b =>
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

                    b.Property<long>("ReferralServiceId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserAccountId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ReferralServiceId");

                    b.HasIndex("UserAccountId");

                    b.ToTable("UserAccountServices");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Organisation", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.ReferralService", null)
                        .WithOne("Organisation")
                        .HasForeignKey("FamilyHubs.Referral.Data.Entities.Organisation", "ReferralServiceId");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.Referral", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.Recipient", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FamilyHubs.Referral.Data.Entities.ReferralService", "ReferralService")
                        .WithMany()
                        .HasForeignKey("ReferralServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FamilyHubs.Referral.Data.Entities.ReferralStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FamilyHubs.Referral.Data.Entities.UserAccount", "ReferralUserAccount")
                        .WithMany()
                        .HasForeignKey("UserAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("ReferralService");

                    b.Navigation("ReferralUserAccount");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccountOrganisation", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FamilyHubs.Referral.Data.Entities.UserAccount", "UserAccount")
                        .WithMany("OrganisationUserAccounts")
                        .HasForeignKey("UserAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");

                    b.Navigation("UserAccount");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccountRole", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FamilyHubs.Referral.Data.Entities.UserAccount", "UserAccount")
                        .WithMany("UserAccountRoles")
                        .HasForeignKey("UserAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("UserAccount");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccountService", b =>
                {
                    b.HasOne("FamilyHubs.Referral.Data.Entities.ReferralService", "ReferralService")
                        .WithMany()
                        .HasForeignKey("ReferralServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FamilyHubs.Referral.Data.Entities.UserAccount", "UserAccount")
                        .WithMany("ServiceUserAccounts")
                        .HasForeignKey("UserAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReferralService");

                    b.Navigation("UserAccount");
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.ReferralService", b =>
                {
                    b.Navigation("Organisation")
                        .IsRequired();
                });

            modelBuilder.Entity("FamilyHubs.Referral.Data.Entities.UserAccount", b =>
                {
                    b.Navigation("OrganisationUserAccounts");

                    b.Navigation("ServiceUserAccounts");

                    b.Navigation("UserAccountRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
