using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHubs.Referral.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyFixesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipients_Referrals_ReferralId",
                table: "Recipients");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferralServices_Referrals_ReferralId",
                table: "ReferralServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Referrers_Referrals_ReferralId",
                table: "Referrers");

            migrationBuilder.DropIndex(
                name: "IX_Referrers_ReferralId",
                table: "Referrers");

            migrationBuilder.DropIndex(
                name: "IX_ReferralServices_ReferralId",
                table: "ReferralServices");

            migrationBuilder.DropIndex(
                name: "IX_Recipients_ReferralId",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "ReferralId",
                table: "Referrers");

            migrationBuilder.DropColumn(
                name: "ReferralId",
                table: "ReferralServices");

            migrationBuilder.DropColumn(
                name: "ReferralId",
                table: "Recipients");

            migrationBuilder.AddColumn<long>(
                name: "RecipientId",
                table: "Referrals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReferralServiceId",
                table: "Referrals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReferrerId",
                table: "Referrals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_RecipientId",
                table: "Referrals",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferralServiceId",
                table: "Referrals",
                column: "ReferralServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_ReferrerId",
                table: "Referrals",
                column: "ReferrerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referrals_Recipients_RecipientId",
                table: "Referrals",
                column: "RecipientId",
                principalTable: "Recipients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Referrals_ReferralServices_ReferralServiceId",
                table: "Referrals",
                column: "ReferralServiceId",
                principalTable: "ReferralServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Referrals_Referrers_ReferrerId",
                table: "Referrals",
                column: "ReferrerId",
                principalTable: "Referrers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Referrals_Recipients_RecipientId",
                table: "Referrals");

            migrationBuilder.DropForeignKey(
                name: "FK_Referrals_ReferralServices_ReferralServiceId",
                table: "Referrals");

            migrationBuilder.DropForeignKey(
                name: "FK_Referrals_Referrers_ReferrerId",
                table: "Referrals");

            migrationBuilder.DropIndex(
                name: "IX_Referrals_RecipientId",
                table: "Referrals");

            migrationBuilder.DropIndex(
                name: "IX_Referrals_ReferralServiceId",
                table: "Referrals");

            migrationBuilder.DropIndex(
                name: "IX_Referrals_ReferrerId",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "ReferralServiceId",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "ReferrerId",
                table: "Referrals");

            migrationBuilder.AddColumn<long>(
                name: "ReferralId",
                table: "Referrers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReferralId",
                table: "ReferralServices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReferralId",
                table: "Recipients",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Referrers_ReferralId",
                table: "Referrers",
                column: "ReferralId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralServices_ReferralId",
                table: "ReferralServices",
                column: "ReferralId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_ReferralId",
                table: "Recipients",
                column: "ReferralId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipients_Referrals_ReferralId",
                table: "Recipients",
                column: "ReferralId",
                principalTable: "Referrals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralServices_Referrals_ReferralId",
                table: "ReferralServices",
                column: "ReferralId",
                principalTable: "Referrals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Referrers_Referrals_ReferralId",
                table: "Referrers",
                column: "ReferralId",
                principalTable: "Referrals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
