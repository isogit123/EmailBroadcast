using Microsoft.EntityFrameworkCore.Migrations;

namespace Emails.Migrations
{
    public partial class UserIdNav : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_UsersId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "UsersId",
                table: "Groups",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_UsersId",
                table: "Groups",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_UsersId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "UsersId",
                table: "Groups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_UsersId",
                table: "Groups",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
