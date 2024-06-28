using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHubs.Referral.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionRequestsSentMetric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectionRequestsSentMetric",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<long>(type: "bigint", nullable: false),
                    UserAccountId = table.Column<long>(type: "bigint", nullable: false),
                    RequestTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestCorrelationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResponseTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HttpResponseCode = table.Column<short>(type: "smallint", nullable: true),
                    ConnectionRequestId = table.Column<long>(type: "bigint", nullable: true),
                    ConnectionRequestReferenceCode = table.Column<string>(type: "nchar(6)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionRequestsSentMetric", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionRequestsSentMetric_RequestCorrelationId",
                table: "ConnectionRequestsSentMetric",
                column: "RequestCorrelationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionRequestsSentMetric");
        }
    }
}
