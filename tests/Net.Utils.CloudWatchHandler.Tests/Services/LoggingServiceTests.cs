using System.Reflection;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Moq;
using Net.Utils.CloudWatchHandler.Models;
using Net.Utils.CloudWatchHandler.Services;
using Newtonsoft.Json;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LoggingServiceTests
{
    private readonly Mock<IAmazonCloudWatchLogs> _mockCloudWatchClient;
    private readonly Mock<ILogStreamService> _mockLogStreamService;
    private const string LogGroupName = "logGroupName";

    public LoggingServiceTests()
    {
        _mockCloudWatchClient = new Mock<IAmazonCloudWatchLogs>();
        _mockLogStreamService = new Mock<ILogStreamService>();
    }

    [Fact]
    public async Task LogMessageAsync_ShouldCall_PutLogEventsAsync()
    {
        var messageDetails = new MessageDetails { ErrorLevel = "ErrorLevel", Message = "Message", ApplicationName = "ApplicationName" };
        var messageData = new MessageData { Prefix = "prefix", DateTimeFormat = "daily", LogGroupName = LogGroupName, MessageDetails = messageDetails };

        var jsonMessageData = JsonConvert.SerializeObject(messageData);

        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("logStreamName");

        var putLogEventsResponse = new PutLogEventsResponse
        {
            NextSequenceToken = "NextSequenceToken123"
        };

        _mockCloudWatchClient.Setup(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ReturnsAsync(putLogEventsResponse);

        var service = new LoggingService(_mockCloudWatchClient.Object, LogGroupName, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonMessageData);

        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldRetry_WhenInvalidSequenceTokenExceptionThrown()
    {
        var messageData = new MessageData { Prefix = "prefix", DateTimeFormat = "daily" };
        var jsonMessageData = JsonConvert.SerializeObject(messageData);

        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.SetupSequence(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ThrowsAsync(new InvalidSequenceTokenException("ExpectedSequenceToken"))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NewSequenceToken" });

        var service = new LoggingService(_mockCloudWatchClient.Object, LogGroupName, _mockLogStreamService.Object);

        var act = async () => await service.LogMessageAsync(jsonMessageData);

        await act.Should().NotThrowAsync<InvalidSequenceTokenException>();
        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Exactly(2));
    }

    [Fact]
    public async Task LogMessageAsync_ShouldUpdateSequenceToken_AfterSuccessfulPut()
    {
        var messageData = new MessageData { Prefix = "prefix", DateTimeFormat = "daily" };
        var jsonMessageData = JsonConvert.SerializeObject(messageData);

        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        const string newSequenceToken = "NewSequenceToken";
        _mockCloudWatchClient.Setup(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = newSequenceToken });

        var service = new LoggingService(_mockCloudWatchClient.Object, LogGroupName, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonMessageData);

        var sequenceTokenField = service.GetType().GetField("_sequenceToken", BindingFlags.NonPublic | BindingFlags.Instance);
        var sequenceTokenValue = sequenceTokenField?.GetValue(service);

        sequenceTokenValue.Should().Be(newSequenceToken);
    }
}