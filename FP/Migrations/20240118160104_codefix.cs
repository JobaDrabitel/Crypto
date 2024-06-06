using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FP.Migrations
{
    /// <inheritdoc />
    public partial class codefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLeader",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Promocodes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promocodes_UserId",
                table: "Promocodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promocodes_Users_UserId",
                table: "Promocodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promocodes_Users_UserId",
                table: "Promocodes");

            migrationBuilder.DropIndex(
                name: "IX_Promocodes_UserId",
                table: "Promocodes");

            migrationBuilder.DropColumn(
                name: "IsLeader",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Promocodes");
        }
    }
}
