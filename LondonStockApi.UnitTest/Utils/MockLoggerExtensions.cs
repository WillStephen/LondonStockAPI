using Microsoft.Extensions.Logging;
using Moq;

namespace LondonStockApi.UnitTest.Utils
{
    public static class MockLoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> mockLogger, LogLevel logLevel, string messageSubstring)
        {
            mockLogger.Verify(l => l.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(messageSubstring)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
        }
    }
}
