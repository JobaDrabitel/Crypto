using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FP.Migrations
{
    /// <inheritdoc />
    public partial class Wallets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Wallets_TopUpWalletId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TopUpWalletId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FromAgentBalance",
                table: "Withdraws");

            migrationBuilder.DropColumn(
                name: "TopUpWalletId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Wallets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WalletType",
                table: "Wallets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DealSum",
                table: "Promocodes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentYield",
                table: "Packs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "WalletType",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "DealSum",
                table: "Promocodes");

            migrationBuilder.DropColumn(
                name: "CurrentYield",
                table: "Packs");

            migrationBuilder.AddColumn<bool>(
                name: "FromAgentBalance",
                table: "Withdraws",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TopUpWalletId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TopUpWalletId",
                table: "Users",
                column: "TopUpWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Wallets_TopUpWalletId",
                table: "Users",
                column: "TopUpWalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
