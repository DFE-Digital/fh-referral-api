using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHubs.Referral.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalProfessionalProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Referrers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Referrers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Referrers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Teams",
                table: "Referrers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForDecliningSupport",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Referrers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Referrers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Referrers");

            migrationBuilder.DropColumn(
                name: "Teams",
                table: "Referrers");

            migrationBuilder.DropColumn(
                name: "ReasonForDecliningSupport",
                table: "Referrals");
        }
    }
}
