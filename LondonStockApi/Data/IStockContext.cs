using LondonStockApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Data
{
    /// <summary>Provides a way to interact with the persistent data store for transactions and quotes.</summary>
    public interface IStockContext
    {
        /// <summary>The store of <see cref="StockTransaction"/> objects.</summary>
        DbSet<StockTransaction> StockTransactions { get; }

        /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)"/>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
