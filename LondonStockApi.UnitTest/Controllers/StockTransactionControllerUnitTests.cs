using AutoFixture;
using AutoMapper;
using LondonStockApi.Controllers;
using LondonStockApi.Data;
using LondonStockApi.Data.Entities;
using LondonStockApi.DTO;
using LondonStockApi.Profiles;
using LondonStockApi.UnitTest.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;

namespace LondonStockApi.UnitTest.Controllers
{
    [TestClass]
    public class StockTransactionControllerUnitTests
    {
        private readonly Fixture fixture = new Fixture();

        private readonly Mock<IStockContext> mockStockContext;
        private readonly Mock<DbSet<StockTransaction>> mockStockTransactionSet;
        private readonly Mock<ILogger<StockTransactionController>> mockLogger;

        private readonly StockTransactionController sut;

        public StockTransactionControllerUnitTests()
        {
            mockStockContext = new Mock<IStockContext>();
            mockStockTransactionSet = new Mock<DbSet<StockTransaction>>();
            mockStockContext.Setup(msc => msc.StockTransactions).Returns(mockStockTransactionSet.Object);
            mockLogger = new Mock<ILogger<StockTransactionController>>();

            IMapper mapper = new MapperConfiguration(mc => mc.AddProfile(new StockTransactionMapping())).CreateMapper();

            sut = new StockTransactionController(mockStockContext.Object, mapper, mockLogger.Object);
        }

        #region Test methods

        [TestMethod, Description("Check that when sending a transaction, and the database persistence has no issues, a 201 code is returned")]
        public async Task Post_ContextHasNoIssues_ReturnsCreated()
        {
            // Arrange
            var dto = fixture.Create<StockTransactionDTO>();

            // Act
            ActionResult<StockTransactionDTO> result = await sut.Post(dto);

            // Assert
            mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Received stock transaction from client");
            mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Persisted stock transaction");

            mockStockContext.Verify(
                sc => sc.StockTransactions.AddAsync(It.Is<StockTransaction>(st => ValuesEqual(dto, st)), default),
                Times.Once);
            mockStockContext.Verify(sc => sc.SaveChangesAsync(default), Times.Once);
            mockStockContext.VerifyNoOtherCalls();

            Assert.IsNull(result.Value);
            StatusCodeResult? statusCodeResult = result.Result as StatusCodeResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual((int)HttpStatusCode.Created, statusCodeResult.StatusCode);
        }

        [TestMethod, Description("Check that when sending a transaction, and the database throws an error, we don't catch that error (so that a 500 code is returned by the framework)")]
        public async Task Post_ContextThrows_Uncaught()
        {
            // Arrange
            var dto = fixture.Create<StockTransactionDTO>();
            mockStockContext
                .Setup(msc => msc.SaveChangesAsync(default))
                .Throws<InvalidOperationException>();

            // Act + Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => sut.Post(dto));

            mockLogger.VerifyLog(LogLevel.Information, messageSubstring: "Received stock transaction from client");
            mockLogger.VerifyNoOtherCalls();
        }

        #endregion Test methods

        #region Support methods

        private static bool ValuesEqual(StockTransactionDTO dto, StockTransaction entity)
        {
            return dto.Ticker == entity.Ticker
                && dto.PriceInSterling == entity.PriceInSterling
                && dto.NumberOfShares == entity.NumberOfShares
                && dto.BrokerId == entity.BrokerId;
        }

        #endregion Support methods
    }
}