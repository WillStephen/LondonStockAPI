using AutoFixture;
using AutoMapper;
using LondonStockApi.Controllers;
using LondonStockApi.Data;
using LondonStockApi.Data.Entities;
using LondonStockApi.DTO;
using LondonStockApi.MappingProfiles;
using LondonStockApi.UnitTest.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LondonStockApi.UnitTest
{
    [TestClass]
    public class QuoteControllerUnitTests
    {
        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IStockContext> mockStockContext;
        private readonly Mock<ILogger<QuoteController>> mockLogger;

        private readonly QuoteController sut;

        public QuoteControllerUnitTests()
        {
            this.mockStockContext = new Mock<IStockContext>();
            this.mockLogger = new Mock<ILogger<QuoteController>>();

            IMapper mapper = new MapperConfiguration(mc => mc.AddProfile(new StockQuoteMapping())).CreateMapper();

            this.sut = new QuoteController(this.mockStockContext.Object, mapper, this.mockLogger.Object);
        }

        #region Test methods

        #region Positive cases

        [TestMethod, Description("Check that when requesting a single quote, and that stock is in the database, the quote is returned")]
        public async Task GetSingleQuote_QuoteInDatabase_ReturnsQuote()
        {
            // Arrange
            string ticker = "NWG";
            StockQuote quote = this.fixture.Create<StockQuote>();
            this.mockStockContext.Setup(sc => sc.TryGetStockQuote(ticker)).ReturnsAsync((true, quote));

            // Act
            ActionResult<StockQuoteDTO> result = await this.sut.GetSingleQuote(ticker);

            // Assert
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Received stock quote request for ticker ");
            this.mockLogger.VerifyNoOtherCalls();

            AssertOKResult(result, expectedTicker: quote.Ticker, expectedPrice: quote.PriceInSterling);
        }

        [TestMethod, Description("Check that when requesting quotes without specifying any tickers, quotes for all stocks are returned from the database")]
        public async Task GetQuotes_NoTickersSpecified_ReturnsAllQuotes()
        {
            // Arrange
            IEnumerable<StockQuote> quotes = this.fixture.CreateMany<StockQuote>(3);
            this.mockStockContext.Setup(sc => sc.GetAllStockQuotes()).ReturnsAsync(quotes);

            // Act
            ActionResult<StockQuoteDTO[]> result = await this.sut.GetQuotes(Array.Empty<string>());

            // Assert
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Received stock quote request for all tickers");
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Retrieved 3 stock quotes from database");
            this.mockLogger.VerifyNoOtherCalls();

            AssertOKResult(result, quotes);
        }

        [DataTestMethod, Description("Check that when requesting quotes for multiple tickers, stocks for all tickers are in the database, quotes are returned for each")]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(10)]
        public async Task GetQuotes_TickersSpecified_ReturnsQuotes(int numberOfTickers)
        {
            // Arrange
            IEnumerable<StockQuote> quotes = this.fixture.CreateMany<StockQuote>(numberOfTickers);
            string[] tickers = quotes.Select(q => q.Ticker).ToArray();

            this.mockStockContext.Setup(sc => sc.TryGetStockQuotes(tickers)).ReturnsAsync((true, quotes));

            // Act
            ActionResult<StockQuoteDTO[]> result = await this.sut.GetQuotes(tickers);

            // Assert
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: $"Received stock quote request for {numberOfTickers} tickers: ");
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: $"Retrieved {numberOfTickers} stock quotes from database");
            this.mockLogger.VerifyNoOtherCalls();

            AssertOKResult(result, quotes);
        }

        #endregion Positive cases

        #region Negative cases

        [TestMethod, Description("Check that when requesting a ticker, and the database doesn't the stock, 404 is returned")]
        public async Task GetSingleQuote_QuoteNotInDatabase_ReturnsNotFound()
        {
            // Arrange
            string ticker = "NWG";
            this.mockStockContext.Setup(sc => sc.TryGetStockQuote(ticker)).ReturnsAsync((false, null));

            // Act
            ActionResult<StockQuoteDTO> result = await this.sut.GetSingleQuote(ticker);

            // Assert
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Received stock quote request for ticker ");
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Stock not found in database");
            this.mockLogger.VerifyNoOtherCalls();

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod, Description("Check that when requesting multiple tickers, and the database doesn't contain all of the stocks, 404 is returned")]
        public async Task GetQuotes_NotAllSpecifiedTickersFound_ReturnsNotFound()
        {
            // Arrange
            IEnumerable<StockQuote> quotes = this.fixture.CreateMany<StockQuote>(3);
            string[] tickers = quotes.Select(q => q.Ticker).ToArray();

            this.mockStockContext.Setup(sc => sc.TryGetStockQuotes(tickers)).ReturnsAsync((false, quotes.Skip(1)));

            // Act
            ActionResult<StockQuoteDTO[]> result = await this.sut.GetQuotes(tickers);

            // Assert
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: $"Received stock quote request for 3 tickers: ");
            this.mockLogger.VerifyLog(LogLevel.Information, messageSubstring: $"Not all stocks found in database");
            this.mockLogger.VerifyNoOtherCalls();

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        #endregion Negative cases

        #endregion Test methods

        #region Support methods

        private void AssertOKResult(
            ActionResult<StockQuoteDTO> result,
            string expectedTicker,
            decimal expectedPrice)
        {
            OkObjectResult? okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            StockQuoteDTO? dto = okResult.Value as StockQuoteDTO;
            Assert.IsNotNull(dto);

            Assert.AreEqual(expectedTicker, dto.Ticker);
            Assert.AreEqual(expectedPrice, dto.PriceInSterling);
        }

        private void AssertOKResult(
            ActionResult<StockQuoteDTO[]> result,
            IEnumerable<StockQuote> quoteEntities)
        {
            OkObjectResult? okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            List<StockQuoteDTO>? dtos = okResult.Value as List<StockQuoteDTO>;
            Assert.IsNotNull(dtos);

            Assert.AreEqual(quoteEntities.Count(), dtos.Count);

            foreach (StockQuote quote in quoteEntities)
            {
                Assert.IsTrue(dtos.FirstOrDefault(dto => 
                    dto.Ticker == quote.Ticker && dto.PriceInSterling == quote.PriceInSterling) != null);
            }
        }

        #endregion Support methods
    }
}
