using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
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

        await service.LogMessageAsync(exceptionData);

        _mockClient.Verify(x => x.PutLogEventsAsync(It.Is<PutLogEventsRequest>(r => r.LogEvents[0].Message == expectedFormattedMessage), default), Times.Once);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldRetry_WhenInvalidSequenceTokenExceptionThrown()
    {
        var exceptionData = new ExceptionData
        {
            ExceptionMessage = "Test Message",
            LogLevel = LogLevel.Info,
            ExceptionType = "SomeException",
            ApplicationName = "SomeApplication",
            Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        _mockLogStreamService.Setup(x => x.CreateLogStreamAsync())
            .ReturnsAsync("TestLogStream");

        _mockClient.SetupSequence(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .Throws(new InvalidSequenceTokenException("Test Exception"))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NewSequenceToken" });

        var service = new LoggingService(_mockClient.Object, TestLogGroupName, _mockLogStreamService.Object);

        var act = async () => await service.LogMessageAsync(exceptionData);

        await act.Should().NotThrowAsync();
        _mockClient.Verify(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Exactly(2));
    }
}