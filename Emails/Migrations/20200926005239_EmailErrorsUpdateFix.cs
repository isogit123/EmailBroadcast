using Microsoft.EntityFrameworkCore.Migrations;

namespace Emails.Migrations
{
    public partial class EmailErrorsUpdateFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentEmailsFailures_SentEmails_SentEmailsId1",
                table: "SentEmailsFailures");

            migrationBuilder.DropIndex(
                name: "IX_SentEmailsFailures_SentEmailsId1",
                table: "SentEmailsFailures");

            migrationBuilder.DropColumn(
                name: "SentEmailsId1",
                table: "SentEmailsFailures");

            migrationBuilder.AlterColumn<string>(
                name: "SentEmailsId",
                table: "SentEmailsFailures",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmailsFailures_SentEmailsId",
                table: "SentEmailsFailures",
                column: "SentEmailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_SentEmailsFailures_SentEmails_SentEmailsId",
                table: "SentEmailsFailures",
                column: "SentEmailsId",
                principalTable: "SentEmails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentEmailsFailures_SentEmails_SentEmailsId",
                table: "SentEmailsFailures");

            migrationBuilder.DropIndex(
                name: "IX_SentEmailsFailures_SentEmailsId",
                table: "SentEmailsFailures");

            migrationBuilder.AlterColumn<int>(
                name: "SentEmailsId",
                table: "SentEmailsFailures",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SentEmailsId1",
                table: "SentEmailsFailures",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SentEmailsFailures_SentEmailsId1",
                table: "SentEmailsFailures",
                column: "SentEmailsId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SentEmailsFailures_SentEmails_SentEmailsId1",
                table: "SentEmailsFailures",
                column: "SentEmailsId1",
                principalTable: "SentEmails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
