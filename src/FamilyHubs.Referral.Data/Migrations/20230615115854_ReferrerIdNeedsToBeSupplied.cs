using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHubs.Referral.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReferrerIdNeedsToBeSupplied : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint in the Referrals table
            migrationBuilder.DropForeignKey(
                name: "FK_Referrals_Referrers_ReferrerId",
                table: "Referrals");

            // Drop the primary key constraint in the Referrers table
            migrationBuilder.DropPrimaryKey(
                name: "PK_Referrers",
                table: "Referrers");

            // Drop the Id column in the Referrers table
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Referrers");

            // Add the Id column back with the desired configuration
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Referrers",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Referrers",
                table: "Referrers",
                column: "Id");

            // Add the foreign key constraint back in the Referrals table
            migrationBuilder.AddForeignKey(
                name: "FK_Referrals_Referrers_ReferrerId",
                table: "Referrals",
                column: "ReferrerId",
                principalTable: "Referrers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint in the Referrals table
            migrationBuilder.DropForeignKey(
                name: "FK_Referrals_Referrers_ReferrerId",
                table: "Referrals");

            // Drop the primary key constraint in the Referrers table
            migrationBuilder.DropPrimaryKey(
                name: "PK_Referrers",
                table: "Referrers");

            // Drop the Id column in the Referrers table
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Referrers");

            // Add the Id column back with the desired configuration
            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Referrers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Referrers",
                table: "Referrers",
                column: "Id");

            // Add the foreign key constraint back in the Referrals table
            migrationBuilder.AddForeignKey(
                name: "FK_Referrals_Referrers_ReferrerId",
                table: "Referrals",
                column: "ReferrerId",
                principalTable: "Referrers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
