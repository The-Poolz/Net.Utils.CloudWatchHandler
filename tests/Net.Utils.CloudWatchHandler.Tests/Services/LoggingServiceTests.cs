using System.Reflection;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
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

    private readonly MessageData _messageData;

    public LoggingServiceTests()
    {
        _mockCloudWatchClient = new Mock<IAmazonCloudWatchLogs>();
        Mock<LogStreamManager> mockLogStreamManager = new();
        _mockLogStreamService = new Mock<LogStreamService>(_mockCloudWatchClient.Object, mockLogStreamManager.Object);
        _loggingService = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        var messageDetails = new MessageDetails { ErrorLevel = "ErrorLevel", Message = "Message", ApplicationName = "ApplicationName" };
        _messageData = new MessageData("prefix", 3, "logGroupName", messageDetails);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldRetry_WhenInvalidSequenceTokenExceptionThrown()
    {
        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.SetupSequence(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ThrowsAsync(new InvalidSequenceTokenException("ExpectedSequenceToken"))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NewSequenceToken" });

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        var act = async () => await service.LogMessageAsync(_messageData);

        await act.Should().NotThrowAsync<InvalidSequenceTokenException>();
        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Exactly(2));
    }

    [Fact]
    public async Task LogMessageAsync_ShouldUpdate_SequenceToken()
    {
        _mockLogStreamService.Setup(x => x.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync("logStreamName");
        _mockCloudWatchClient.Setup(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), CancellationToken.None))
            
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "newToken" });

        await _loggingService.LogMessageAsync(_messageData);

        _loggingService.GetType().GetField("_sequenceToken", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(_loggingService).Should().Be("newToken");
    }
    
    [Fact]
    public async Task LogMessageAsync_ShouldLogSuccessfully()
    {
        _mockLogStreamService.Setup(x => x.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.SetupSequence(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), CancellationToken.None))
            .ThrowsAsync(new InvalidSequenceTokenException("Invalid token"))
            .ReturnsAsync(new PutLogEventsResponse
            {
                NextSequenceToken = "nextSequenceToken"
            });

        await _loggingService.LogMessageAsync(_messageData);

        _mockCloudWatchClient.Verify(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), CancellationToken.None), Times.Exactly(2));
    }
}