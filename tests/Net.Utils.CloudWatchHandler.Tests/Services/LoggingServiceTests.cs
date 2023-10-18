using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Moq;
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
        const string logLevel = "Error";
        const string exceptionType = "DownloaderException";
        const string applicationName = "LambdaSet";
        const string message = "TestMessage";

        const string? jsonData = $"{{\"LogLevel\":\"{logLevel}\",\"ExceptionType\":\"{exceptionType}\",\"ApplicationName\":\"{applicationName}\",\"Message\":\"{message}\"}}";

        var expectedFormattedMessage = MessageFormatter.FormatExceptionMessage(jsonData);

        _mockLogStreamService.Setup(x => x.CreateLogStreamAsync())
            .ReturnsAsync("TestLogStream");

        _mockClient.Setup(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ReturnsAsync(new PutLogEventsResponse());

        var service = new LoggingService(_mockClient.Object, TestLogGroupName, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonData);

        _mockClient.Verify(x => x.PutLogEventsAsync(It.Is<PutLogEventsRequest>(r => r.LogEvents[0].Message == expectedFormattedMessage), default), Times.Once);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldRetry_WhenInvalidSequenceTokenExceptionThrown()
    {
        const string logLevel = "Error";
        const string exceptionType = "DownloaderException";
        const string applicationName = "LambdaSet";
        const string message = "TestMessage";

        const string? jsonData = $"{{\"LogLevel\":\"{logLevel}\",\"ExceptionType\":\"{exceptionType}\",\"ApplicationName\":\"{applicationName}\",\"Message\":\"{message}\"}}";

        _mockLogStreamService.Setup(x => x.CreateLogStreamAsync())
            .ReturnsAsync("TestLogStream");

        _mockClient.SetupSequence(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .Throws(new InvalidSequenceTokenException("Test Exception"))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NewSequenceToken" });

        var service = new LoggingService(_mockClient.Object, TestLogGroupName, _mockLogStreamService.Object);

        var act = async () => await service.LogMessageAsync(jsonData);

        await act.Should().NotThrowAsync();
        _mockClient.Verify(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Exactly(2));
    }
}