using LondonStockApi.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LondonStockApi.Data
{
    /// <inheritdoc/>
    public class StockContext : DbContext, IStockContext
    {
        private readonly IConfiguration configuration;

        /// <inheritdoc/>
        public DbSet<StockTransaction> StockTransactions { get; set; }

        private DbSet<StockQuote> StockQuotes { get; set; }

        public StockContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<(bool Found, StockQuote? Quote)> TryGetStockQuote(string ticker)
        {
            IEnumerable<StockQuote> quotes = await this.StockQuotes
                .FromSqlInterpolated($"GetPriceForTicker {ticker}").ToListAsync();

            if (!quotes.Any())
            {
                return (false, null);
            }

            if (quotes.Count() > 1)
            {
                throw new InvalidDataException("Expected no more than one result from the database.");
            }

            return (true, quotes.Single());
        }

        /// <inheritdoc/>
        public async Task<(bool FoundAll, IEnumerable<StockQuote>? Quotes)> TryGetStockQuotes(string[] tickers)
        {
            SqlParameter tickerQueryParameter = CreateTickerQueryParameter(tickers);

            var quotes = await this.StockQuotes
                .FromSqlInterpolated($"GetPriceForTickers {tickerQueryParameter}").ToListAsync();

            // if any of the requested tickers are missing in the results, fail the whole operation
            // ensures the erroneous client code doesn't fail silently
            if (quotes.Count < tickers.Length)
            {
                return (false, null);
            }

            if (quotes.Count > tickers.Length)
            {
                throw new InvalidDataException("Expected result row count to be less than or equal to ticker count.");
            }

            return (true, quotes);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<StockQuote>> GetAllStockQuotes() =>
            await this.StockQuotes.FromSqlInterpolated($"GetPriceForAllTickers").ToListAsync();

        protected override void OnConfiguring(DbContextOptionsBuilder options) => 
            options.UseSqlServer(this.configuration.GetConnectionString("LondonStockApi"));

        #region Support methods

        private SqlParameter CreateTickerQueryParameter(string[] tickers)
        {
            // create a temporary table to pass values to the stored procedure
            var tickerQueryTable = new DataTable();
            tickerQueryTable.Columns.Add("Ticker", typeof(string));
            foreach (var ticker in tickers)
            {
                tickerQueryTable.Rows.Add(ticker);
            }

            var parameter = new SqlParameter("@TickerQuery", SqlDbType.Structured);
            parameter.Value = tickerQueryTable;
            parameter.TypeName = "dbo.TickerQuery";
            return parameter;
        }

        #endregion Support methods
    }
}
