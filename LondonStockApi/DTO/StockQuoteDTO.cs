namespace LondonStockApi.DTO
{
    /// <summary>
    /// A quote for a particular security.
    /// </summary>
    /// <param name="Ticker">Unique ticker symbol for the security.</param>
    /// <param name="PriceInSterling">Price per share in pound sterling.</param>
    public record StockQuoteDTO(
        string Ticker,
        decimal PriceInSterling);
}
