using LondonStockApi.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LondonStockApi.UnitTest.Validation
{
    /// <remarks>We already have<see cref="StockTickerAttributeUnitTests"/>, so no need to overboard here.</remarks>
    [TestClass]
    public class StockTickerCollectionAttributeUnitTests
    {
        [DataTestMethod]
        [DataRow()]
        [DataRow("BRK.B")]
        [DataRow("BRK.B", "NWG", "VWRP")]
        public void IsValid_ValidStrings_ReturnsTrue(params string[] testStrings)
        {
            // Arrange
            var sut = new StockTickerCollectionAttribute();

            // Act
            bool result = sut.IsValid(testStrings);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("BRK.5")]
        [DataRow(3, ".", null)]
        public void IsValid_ValidObjects_ReturnsFalse(params object[] testObjects)
        {
            // Arrange
            var sut = new StockTickerCollectionAttribute();

            // Act
            bool result = sut.IsValid(testObjects);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
