using Microsoft.EntityFrameworkCore.Migrations;

namespace Emails.Migrations
{
    public partial class EmailErrorsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailErrors_SentEmailsErrors_SentEmailsErrorsId",
                table: "EmailErrors");

            migrationBuilder.DropIndex(
                name: "IX_EmailErrors_SentEmailsErrorsId",
                table: "EmailErrors");

            migrationBuilder.DropColumn(
                name: "SentEmailsErrorsId",
                table: "EmailErrors");

            migrationBuilder.CreateTable(
                name: "SentEmailsErrorsEmailErrors",
                columns: table => new
                {
                    SentEmailsErrorsId = table.Column<int>(nullable: false),
                    EmailErrorsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmailsErrorsEmailErrors", x => new { x.SentEmailsErrorsId, x.EmailErrorsId });
                    table.ForeignKey(
                        name: "FK_SentEmailsErrorsEmailErrors_EmailErrors_EmailErrorsId",
                        column: x => x.EmailErrorsId,
                        principalTable: "EmailErrors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SentEmailsErrorsEmailErrors_SentEmailsErrors_SentEmailsErro~",
                        column: x => x.SentEmailsErrorsId,
                        principalTable: "SentEmailsErrors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SentEmailsErrorsEmailErrors_EmailErrorsId",
                table: "SentEmailsErrorsEmailErrors",
                column: "EmailErrorsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SentEmailsErrorsEmailErrors");

            migrationBuilder.AddColumn<int>(
                name: "SentEmailsErrorsId",
                table: "EmailErrors",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailErrors_SentEmailsErrorsId",
                table: "EmailErrors",
                column: "SentEmailsErrorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailErrors_SentEmailsErrors_SentEmailsErrorsId",
                table: "EmailErrors",
                column: "SentEmailsErrorsId",
                principalTable: "SentEmailsErrors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
