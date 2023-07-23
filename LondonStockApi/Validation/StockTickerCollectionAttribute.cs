namespace LondonStockApi.Validation
{
    /// <summary>Validates collections of strings representing stock tickers.</summary>
    public class StockTickerCollectionAttribute : StockTickerAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || value is not IEnumerable<string> valueStrings)
            {
                return false;
            }

            return valueStrings.All(vs => base.IsValid(vs));
        }
    }
}
