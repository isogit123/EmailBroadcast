using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Emails.Migrations
{
    public partial class EmailErrors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SentEmails",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    SendingDate = table.Column<DateTime>(nullable: false),
                    GroupsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SentEmails_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SentEmailsErrors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Recipient = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmailsErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailErrors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ErrorMsg = table.Column<string>(nullable: true),
                    SentEmailsErrorsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailErrors_SentEmailsErrors_SentEmailsErrorsId",
                        column: x => x.SentEmailsErrorsId,
                        principalTable: "SentEmailsErrors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailErrors_SentEmailsErrorsId",
                table: "EmailErrors",
                column: "SentEmailsErrorsId");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_GroupsId",
                table: "SentEmails",
                column: "GroupsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailErrors");

            migrationBuilder.DropTable(
                name: "SentEmails");

            migrationBuilder.DropTable(
                name: "SentEmailsErrors");
        }
    }
}
