﻿// <auto-generated />
using System;
using FamilyHubs.ReferralApi.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FamilyHubs.ReferralApi.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FamilyHubs.ReferralApi.Core.Entities.Referral", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Created")
                        .IsRequired()
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime?>("DateRecieved")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HasSpecialNeeds")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<string>("OrganisationId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("ReasonForRejection")
                        .HasColumnType("text");

                    b.Property<string>("ReasonForSupport")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Referrer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("RequestNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("RequestNumber"));

                    b.Property<string>("ServiceAsJson")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ServiceDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ServiceId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Referrals");
                });

            modelBuilder.Entity("FamilyHubs.ReferralApi.Core.Entities.ReferralStatus", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<string>("ReferralId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ReferralId");

                    b.ToTable("ReferralStatuses");
                });

            modelBuilder.Entity("FamilyHubs.ReferralApi.Core.Entities.ReferralStatus", b =>
                {
                    b.HasOne("FamilyHubs.ReferralApi.Core.Entities.Referral", null)
                        .WithMany("Status")
                        .HasForeignKey("ReferralId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FamilyHubs.ReferralApi.Core.Entities.Referral", b =>
                {
                    b.Navigation("Status");
                });
#pragma warning restore 612, 618
        }
    }
}
