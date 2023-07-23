using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonStockApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateStocksDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "varchar(255)", nullable: false),
                    PriceInSterling = table.Column<decimal>(type: "decimal(19,5)", nullable: false),
                    NumberOfShares = table.Column<decimal>(type: "decimal(19,5)", nullable: false),
                    BrokerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockTransactions");
        }
    }
}
