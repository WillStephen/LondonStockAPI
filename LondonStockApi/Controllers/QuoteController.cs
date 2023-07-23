using AutoMapper;
using LondonStockApi.Data;
using LondonStockApi.Data.Entities;
using LondonStockApi.DTO;
using LondonStockApi.Validation;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuoteController : ControllerBase
    {
        private readonly IStockContext stockContext;
        private readonly IMapper mapper;
        private readonly ILogger<QuoteController> logger;

        public QuoteController(
            IStockContext stockContext,
            IMapper mapper,
            ILogger<QuoteController> logger)
        {
            this.stockContext = stockContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Get the price of a security from the exchange.
        /// </summary>
        [HttpGet]
        [Route("{ticker}")]
        public async Task<ActionResult<StockQuoteDTO>> GetSingleQuote([StockTicker] string ticker)
        {
            this.logger.LogInformation("Received stock quote request for ticker {ticker}", ticker);

            (bool found, StockQuote? quote) = await this.stockContext.TryGetStockQuote(ticker);
            if (!found)
            {
                this.logger.LogInformation("Stock not found in database");
                return NotFound();
            }

            return Ok(this.mapper.Map<StockQuoteDTO>(quote!));
        }

        /// <summary>
        /// Get the price of securities with tickers specified in <paramref name="tickers"/> from the exchange.
        /// </summary>
        /// <param name="tickers">The list of tickers to retrieve quotes for. If empty, returns all quotes.</param>
        [HttpGet]
        public async Task<ActionResult<StockQuoteDTO[]>> GetQuotes([FromQuery][StockTickerCollection] string[] tickers)
        {
            IEnumerable<StockQuote> quotes;

            if (tickers.Length == 0) // if no tickers provided, return all stock quotes
            {
                this.logger.LogInformation("Received stock quote request for all tickers");

                quotes = await this.stockContext.GetAllStockQuotes();
            }
            else
            {
                this.logger.LogInformation("Received stock quote request for {tickerCount} tickers: {tickers}",
                    tickers.Length,
                    (object)tickers);

                (bool foundAll, IEnumerable<StockQuote>? retrievedQuotes) = await this.stockContext.TryGetStockQuotes(tickers);
                if (!foundAll)
                {
                    this.logger.LogInformation("Not all stocks found in database");
                    return NotFound();
                }

                quotes = retrievedQuotes!;
            }

            this.logger.LogInformation("Retrieved {count} stock quotes from database", quotes.Count());
            return Ok(this.mapper.Map<List<StockQuoteDTO>>(quotes));
        }
    }
}
