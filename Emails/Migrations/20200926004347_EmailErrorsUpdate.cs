using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Emails.Migrations
{
    public partial class EmailErrorsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SentEmailsErrorsEmailErrors");

            migrationBuilder.DropTable(
                name: "EmailErrors");

            migrationBuilder.DropTable(
                name: "SentEmailsErrors");

            migrationBuilder.CreateTable(
                name: "SentEmailsFailures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Recipient = table.Column<string>(nullable: true),
                    SentEmailsId = table.Column<int>(nullable: false),
                    SentEmailsId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmailsFailures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SentEmailsFailures_SentEmails_SentEmailsId1",
                        column: x => x.SentEmailsId1,
                        principalTable: "SentEmails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SentEmailsFailures_SentEmailsId1",
                table: "SentEmailsFailures",
                column: "SentEmailsId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SentEmailsFailures");

            migrationBuilder.CreateTable(
                name: "EmailErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ErrorMsg = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SentEmailsErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Recipient = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmailsErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SentEmailsErrorsEmailErrors",
                columns: table => new
                {
                    SentEmailsErrorsId = table.Column<int>(type: "integer", nullable: false),
                    EmailErrorsId = table.Column<int>(type: "integer", nullable: false)
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
    }
}
