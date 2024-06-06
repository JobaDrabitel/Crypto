using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FP.Migrations
{
    /// <inheritdoc />
    public partial class DeFiTransactionsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccrualInFound",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "AdditionalPercentage",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "DaysWithoutWithdraws",
                table: "Investments");

            migrationBuilder.CreateTable(
                name: "DefiTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdditionPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    Sum = table.Column<decimal>(type: "numeric", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    DaysWithoutWithdraws = table.Column<int>(type: "integer", nullable: false),
                    InvestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefiTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefiTransactions_Investments_InvestId",
                        column: x => x.InvestId,
                        principalTable: "Investments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefiTransactions_InvestId",
                table: "DefiTransactions",
                column: "InvestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefiTransactions");

            migrationBuilder.AddColumn<decimal>(
                name: "AccrualInFound",
                table: "Investments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AdditionalPercentage",
                table: "Investments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DaysWithoutWithdraws",
                table: "Investments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
