using System.ComponentModel.DataAnnotations;

namespace LondonStockApi.DTO
{
    /// <summary>
    /// A record of a transaction of shares on the exchange.
    /// </summary>
    /// <param name="Ticker">Unique ticker symbol for the security.</param>
    /// <param name="PriceInSterling">Price per share in pound sterling.</param>
    /// <param name="NumberOfShares">Number of shares traded in the transaction.</param>
    /// <param name="BrokerId">Unique ID of the broker that requested this transaction.</param>
    public record StockTransactionDTO(
        [Required] 
        string Ticker,

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = $"{nameof(PriceInSterling)} must be positive.")]
        decimal PriceInSterling,

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = $"{nameof(NumberOfShares)} must be positive.")]
        decimal NumberOfShares,

        [Required] 
        Guid BrokerId);
}
