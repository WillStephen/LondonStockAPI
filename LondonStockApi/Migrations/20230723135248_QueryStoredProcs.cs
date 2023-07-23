using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonStockApi.Migrations
{
    /// <inheritdoc />
    public partial class QueryStoredProcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(GetPriceForTickerProc);
            migrationBuilder.Sql(CreateTickerQueryType);
            migrationBuilder.Sql(GetPriceForTickersProc);
            migrationBuilder.Sql(GetPriceForAllTickersProc);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP TYPE [dbo].[TickerQuery]" +
                "DROP PROCEDURE [dbo].[GetPriceForTicker] GO " +
                "DROP PROCEDURE [dbo].[GetPriceForTickers] GO " +
                "DROP PROCEDURE [dbo].[GetPriceForAllTickers] GO ");
        }

        private string GetPriceForTickerProc =>
            "CREATE OR ALTER PROCEDURE [dbo].[GetPriceForTicker] @Ticker nvarchar(255) AS " +
                "SELECT @Ticker, AVG(PriceInSterling) " +
                "FROM dbo.StockTransactions " +
                "WHERE Ticker = @Ticker GROUP BY Ticker " +
            "\r\nGO ";

        private string CreateTickerQueryType =>
            "CREATE TYPE dbo.TickerQuery AS " +
            "TABLE(Ticker nvarchar(255)); " +
            "\r\nGO ";

        private string GetPriceForTickersProc =>
            "CREATE OR ALTER PROCEDURE [dbo].[GetPriceForTickers] @TickerList AS dbo.TickerList READONLY AS " +
                "SELECT tl.Ticker, AVG(StockTransactions.PriceInSterling) " +
                "FROM @TickerList tl " +
                "INNER JOIN StockTransactions " +
                "ON tl.Ticker = StockTransactions.Ticker " +
                "GROUP BY tl.Ticker " +
            "\r\nGO ";

        private string GetPriceForAllTickersProc =>
            "CREATE PROCEDURE [dbo].[GetPriceForAllTickers] AS " +
                "SELECT Ticker, AVG(PriceInSterling) " +
                "FROM dbo.StockTransactions " +
                "GROUP BY Ticker " +
            "\r\nGO ";
    }
}
