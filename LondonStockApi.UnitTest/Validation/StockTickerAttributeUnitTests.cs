using LondonStockApi.Validation;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace LondonStockApi.UnitTest.Validation
{
    [TestClass]
    public class StockTickerAttributeUnitTests
    {
        [DataTestMethod]
        [DataRow("VWRL")]
        [DataRow("RDSA.A")]
        [DataRow("NWG")]
        [DataRow("A")]
        [DataRow("BRK.B")]
        
        public void IsValid_ValidString_ReturnsTrue(string testString)
        {
            // Arrange
            var sut = new StockTickerAttribute();

            // Act
            bool result = sut.IsValid(testString);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow("vwrl")]
        [DataRow("VwRl")]
        [DataRow(".")]
        [DataRow("BRK.5")]
        [DataRow("BRK.b")]
        [DataRow("brk.b")]
        [DataRow("-")]
        [DataRow("?")]
        [DataRow("?")]
        public void IsValid_InvalidObject_ReturnsFalse(object? testObject)
        {
            // Arrange
            var sut = new StockTickerAttribute();

            // Act
            bool result = sut.IsValid(testObject);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual("Tickers must be consist of upper-case alphabetical and numeric characters, and '.' if necessary.", sut.ErrorMessage);
        }

        [TestMethod]
        public void IsValid_TickerTooLong_ReturnsFalse()
        {
            // Arrange
            string testString = "TOOOOOOOOOOLOOOOOOOOOONG";
            var sut = new StockTickerAttribute();

            // Act
            bool result = sut.IsValid(testString);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual("Tickers must be between 1 and 14 characters in length.", sut.ErrorMessage);
        }

        [DataTestMethod]
        [DataRow(3)]
        [DataRow(null)]
        [DataRow("")]

        public void IsValid_TickerWrongTypeOrEmpty_ReturnsFalse(object? testObject)
        {
            // Arrange
            var sut = new StockTickerAttribute();

            // Act
            bool result = sut.IsValid(testObject);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual("Tickers must be non-empty strings.", sut.ErrorMessage);
        }

        #region Support methods

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        #endregion Support methods
    }
}
