using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LondonStockApi.Validation
{
    /// <summary>Validates string fields representing stock tickers.</summary>
    public partial class StockTickerAttribute : ValidationAttribute
    {
        private const int MaxLength = 14;

        public override bool IsValid(object? value)
        {
            if (value == null || value is not string valueString || valueString.Length == 0)
            {
                ErrorMessage = $"Tickers must be non-empty strings.";
                return false;
            }

            if (valueString.Length > MaxLength)
            {
                ErrorMessage = $"Tickers must be between 1 and {MaxLength} characters in length.";
                return false;
            }

            if (!TickerRegex().IsMatch(valueString))
            {
                ErrorMessage = $"Tickers must be consist of upper-case alphabetical and numeric characters, and '.' if necessary.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// This regex checks that the string contains only upper-case alphabetical and numeric 
        /// characters and '.'. Some tickers can have '.' e.g. Berkshire Hathaway uses 'BRK.B'. 
        /// In this case we check that there is exactly one upper-case alphabetical character 
        /// after the '.'.
        /// </summary>
        [GeneratedRegex("^[A-Z0-9]+(\\.[A-Z]{1})?$")]
        private static partial Regex TickerRegex();
    }
}
