using System.Reflection;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Net.Utils.CloudWatchHandler.Helpers;
using Net.Utils.CloudWatchHandler.Models;
using Net.Utils.CloudWatchHandler.Services;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LoggingServiceTests
{
    private readonly Mock<IAmazonCloudWatchLogs> _mockCloudWatchClient;
    private readonly Mock<LogStreamService> _mockLogStreamService;
    private readonly LoggingService _loggingService;
    private readonly LogConfiguration _logConfiguration;

    public LoggingServiceTests()
    {
        var messageDetails = new MessageDetails(LogLevel.Error, "message", "LambdaSet");
        _logConfiguration = new LogConfiguration("prefix", 5, "logGroupName", messageDetails);

        _mockCloudWatchClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamManager = new Mock<LogStreamManager>();
        _mockLogStreamService = new Mock<LogStreamService>(_mockCloudWatchClient.Object, mockLogStreamManager.Object);
        _loggingService = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldRetryAndSucceed_WhenInvalidToken()
    {
        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.SetupSequence(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ThrowsAsync(new InvalidSequenceTokenException("Expected"))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NewToken" });

        await _loggingService.Invoking(y => y.LogMessageAsync(_logConfiguration))
            .Should().NotThrowAsync<InvalidSequenceTokenException>();

        _mockCloudWatchClient.Verify(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Exactly(2));
    }

    [Fact]
    public async Task LogMessageAsync_ShouldUpdate_SequenceToken()
    {
        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.Setup(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NewToken" });

        await _loggingService.LogMessageAsync(_logConfiguration);

        var sequenceToken = _loggingService.GetType().GetField("_sequenceToken", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(_loggingService);
        sequenceToken.Should().Be("NewToken");
    }

    [Fact]
    public async Task LogMessageAsync_ShouldLogSuccessfully()
    {
        _mockLogStreamService.Setup(x => x.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.SetupSequence(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ThrowsAsync(new InvalidSequenceTokenException("Invalid token"))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "nextSequenceToken" });

        await _loggingService.Invoking(y => y.LogMessageAsync(_logConfiguration))
            .Should().NotThrowAsync<InvalidSequenceTokenException>();

        _mockCloudWatchClient.Verify(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Exactly(2));
    }
}
