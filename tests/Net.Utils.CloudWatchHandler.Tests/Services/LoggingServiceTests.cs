using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Moq;
using Net.Utils.CloudWatchHandler.Models;
using Net.Utils.CloudWatchHandler.Services;
using Net.Utils.CloudWatchHandler.Utilities;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LoggingServiceTests
{
    private readonly Mock<IAmazonCloudWatchLogs> _mockClient = new();
    private readonly Mock<LogStreamService> _mockLogStreamService = new(Mock.Of<IAmazonCloudWatchLogs>(), TestLogGroupName);
    private const string TestLogGroupName = "TestLogGroup";

    [Fact]
    public async Task LogMessageAsync_ShouldLogFormattedMessage()
    {
        var exceptionData = new ExceptionData
        {
            LogLevel = LogLevel.Error,
            ExceptionType = "DownloaderException",
            ApplicationName = "LambdaSet",
            ExceptionMessage = "TestMessage",
            Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        var expectedFormattedMessage = MessageFormatter.FormatExceptionMessage(exceptionData);

        _mockLogStreamService.Setup(x => x.CreateLogStreamAsync())
            .ReturnsAsync("TestLogStream");

        _mockClient.Setup(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ReturnsAsync(new PutLogEventsResponse());

        var service = new LoggingService(_mockClient.Object, TestLogGroupName, _mockLogStreamService.Object);

        // Act
        await service.LogMessageAsync(exceptionData);

        // Assert
        _mockClient.Verify(x => x.PutLogEventsAsync(It.Is<PutLogEventsRequest>(r => r.LogEvents[0].Message == expectedFormattedMessage), default), Times.Once);
    }
}