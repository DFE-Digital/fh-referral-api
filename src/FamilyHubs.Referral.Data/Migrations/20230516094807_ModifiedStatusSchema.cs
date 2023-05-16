using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHubs.Referral.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedStatusSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ReferralStatuses", true);
            migrationBuilder.Sql("DELETE FROM ReferralOrganisations", true);
            migrationBuilder.Sql("DELETE FROM ReferralServices", true);
            migrationBuilder.Sql("DELETE FROM Recipients", true);
            migrationBuilder.Sql("DELETE FROM Referrers", true);
            migrationBuilder.Sql("DELETE FROM Referrals", true);

            migrationBuilder.DropForeignKey(
                name: "FK_ReferralStatuses_Referrals_ReferralId",
                table: "ReferralStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ReferralStatuses_ReferralId",
                table: "ReferralStatuses");

            migrationBuilder.DropColumn(
                name: "ReferralId",
                table: "ReferralStatuses");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ReferralStatuses",
                newName: "Name");

            migrationBuilder.AddColumn<byte>(
                name: "SortOrder",
                table: "ReferralStatuses",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "Referrals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_StatusId",
                table: "Referrals",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referrals_ReferralStatuses_StatusId",
                table: "Referrals",
                column: "StatusId",
                principalTable: "ReferralStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Referrals_ReferralStatuses_StatusId",
                table: "Referrals");

            migrationBuilder.DropIndex(
                name: "IX_Referrals_StatusId",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "ReferralStatuses");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Referrals");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ReferralStatuses",
                newName: "Status");

            migrationBuilder.AddColumn<long>(
                name: "ReferralId",
                table: "ReferralStatuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralStatuses_ReferralId",
                table: "ReferralStatuses",
                column: "ReferralId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralStatuses_Referrals_ReferralId",
                table: "ReferralStatuses",
                column: "ReferralId",
                principalTable: "Referrals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
