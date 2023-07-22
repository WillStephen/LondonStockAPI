using LondonStockApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Data
{
    /// <inheritdoc/>
    public class StockContext : DbContext, IStockContext
    {
        private readonly IConfiguration configuration;

        public StockContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => 
            options.UseSqlServer(this.configuration.GetConnectionString("LondonStockApi"));

        /// <inheritdoc/>
        public DbSet<StockTransaction> StockTransactions { get; set; }
    }
}
