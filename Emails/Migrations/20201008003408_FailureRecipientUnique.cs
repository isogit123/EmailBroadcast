using Microsoft.EntityFrameworkCore.Migrations;

namespace Emails.Migrations
{
    public partial class FailureRecipientUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SentEmailsFailures_Recipient_SentEmailsId",
                table: "SentEmailsFailures",
                columns: new[] { "Recipient", "SentEmailsId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SentEmailsFailures_Recipient_SentEmailsId",
                table: "SentEmailsFailures");
        }
    }
}
