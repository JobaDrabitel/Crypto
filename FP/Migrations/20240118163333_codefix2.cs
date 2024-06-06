using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FP.Migrations
{
    /// <inheritdoc />
    public partial class codefix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promocodes_Users_UserId",
                table: "Promocodes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Promocodes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActived",
                table: "Promocodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Promocodes_Users_UserId",
                table: "Promocodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promocodes_Users_UserId",
                table: "Promocodes");

            migrationBuilder.DropColumn(
                name: "IsActived",
                table: "Promocodes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Promocodes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Promocodes_Users_UserId",
                table: "Promocodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
