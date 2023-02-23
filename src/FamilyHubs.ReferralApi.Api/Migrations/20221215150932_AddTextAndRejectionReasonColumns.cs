using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyHubs.ReferralApi.Api.Migrations
{
    public partial class AddTextAndRejectionReasonColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferralStatuses_Referrals_ReferralId",
                table: "ReferralStatuses");

            migrationBuilder.AlterColumn<string>(
                name: "ReferralId",
                table: "ReferralStatuses",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Referrals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Referrals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ReasonForRejection",
                table: "Referrals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Referrals",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralStatuses_Referrals_ReferralId",
                table: "ReferralStatuses",
                column: "ReferralId",
                principalTable: "Referrals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferralStatuses_Referrals_ReferralId",
                table: "ReferralStatuses");

            migrationBuilder.DropColumn(
                name: "ReasonForRejection",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Referrals");

            migrationBuilder.AlterColumn<string>(
                name: "ReferralId",
                table: "ReferralStatuses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Referrals",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Referrals",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralStatuses_Referrals_ReferralId",
                table: "ReferralStatuses",
                column: "ReferralId",
                principalTable: "Referrals",
                principalColumn: "Id");
        }
    }
}
