using LondonStockApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Data
{
    /// <summary>Provides a way to interact with the persistent data store for transactions and quotes.</summary>
    public interface IStockContext
    {
        /// <summary>The store of <see cref="StockTransaction"/> objects.</summary>
        DbSet<StockTransaction> StockTransactions { get; }

        /// <summary>
        /// Try to retrieve a quote for a single ticker.
        /// </summary>
        /// <param name="ticker">The ticker to search for.</param>
        /// <returns>A <see cref="bool"/> representing whether the value was in the database, and a <see cref="StockQuote"/>
        /// (<see langword="null"/> if not found).</returns>
        Task<(bool Found, StockQuote? Quote)> TryGetStockQuote(string ticker);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tickers">The tickers to search for.</param>
        /// <returns>A <see cref="bool"/> representing whether the value was in the DB, and <see cref="StockQuote"/>s 
        /// (<see langword="null"/> if not found).</returns>
        Task<(bool FoundAll, IEnumerable<StockQuote>? Quotes)> TryGetStockQuotes(string[] tickers);

        /// <summary>
        /// Get quotes for all tickers in the database.
        /// </summary>
        Task<IEnumerable<StockQuote>> GetAllStockQuotes();

        /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)"/>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
