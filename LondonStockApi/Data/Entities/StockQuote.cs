using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LondonStockApi.Data.Entities
{
    /// <summary>
    /// An invidual stock quote in the database.
    /// </summary>
    /// <remarks>
    /// No need for a <see cref="TableAttribute"/> - this is a virtual table returned by a stored procedure.
    /// </remarks>
    public class StockQuote
    {
        [Key]
        public required string Ticker { get; init; }

        public decimal PriceInSterling { get; init; }
    }
}
