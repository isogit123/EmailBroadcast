using Microsoft.EntityFrameworkCore.Migrations;

namespace Emails.Migrations
{
    public partial class FixGroupId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Groups_GroupsId",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Emails");

            migrationBuilder.AlterColumn<int>(
                name: "GroupsId",
                table: "Emails",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Groups_GroupsId",
                table: "Emails",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Groups_GroupsId",
                table: "Emails");

            migrationBuilder.AlterColumn<int>(
                name: "GroupsId",
                table: "Emails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Emails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Groups_GroupsId",
                table: "Emails",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
