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
    private readonly Mock<LogStreamService> _mockLogStreamService;
    private readonly MessageData _messageData;

    public LoggingServiceTests()
    {
        _mockCloudWatchClient = new Mock<IAmazonCloudWatchLogs>();
        _mockLogStreamService = new Mock<LogStreamService>();
        var messageDetails = new MessageDetails { ErrorLevel = "ErrorLevel", Message = "Message", ApplicationName = "ApplicationName" };
        _messageData = new MessageData("prefix", 3, "logGroupName", messageDetails);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldThrowException_WhenMessageDetailsIsNull()
    {
        _messageData.MessageDetails = null;

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await Assert.ThrowsAsync<System.InvalidOperationException>(() => service.LogMessageAsync(_messageData));
    }

    [Fact]
    public async Task LogMessageAsync_ShouldThrowException_WhenLogGroupNameIsIncorrect()
    {
        JsonConvert.SerializeObject(_messageData);
        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await Assert.ThrowsAsync<System.InvalidOperationException>(() => service.LogMessageAsync(null));
    }
/*
    [Fact]
    public async Task LogMessageAsync_ShouldThrowException_WhenDateTimeFormatIsIncorrect()
    {
        _messageData.DateTimeFormat = "incorrectFormat";
        var jsonMessageData = JsonConvert.SerializeObject(_messageData);
        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new FormatException());

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await Assert.ThrowsAsync<FormatException>(() => service.LogMessageAsync(jsonMessageData));
    }

    private void SetupDefaultMocks()
    {
        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.Setup(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NextSequenceToken123" });
    }

    [Fact]
    public async Task LogMessageAsync_WhenMessageDetailsAreValid_ShouldLogSuccessfully()
    {
        var jsonMessageData = JsonConvert.SerializeObject(_messageData);

        SetupDefaultMocks();

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonMessageData);

        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task LogMessageAsync_WhenLogGroupNameIsCorrect_ShouldLogSuccessfully()
    {
        _messageData.LogGroupName = "CorrectLogGroupName";
        var jsonMessageData = JsonConvert.SerializeObject(_messageData);

        SetupDefaultMocks();

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonMessageData);

        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task LogMessageAsync_WhenDateTimeFormatIsCorrect_ShouldLogSuccessfully()
    {
        _messageData.DateTimeFormat = "yyyy-MM-dd";
        var jsonMessageData = JsonConvert.SerializeObject(_messageData);

        SetupDefaultMocks();

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonMessageData);

        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldCall_PutLogEventsAsync()
    {
        var jsonMessageData = JsonConvert.SerializeObject(_messageData);

        SetupDefaultMocks();

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonMessageData);

        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldRetry_WhenInvalidSequenceTokenExceptionThrown()
    {
        var jsonMessageData = JsonConvert.SerializeObject(_messageData);

        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        _mockCloudWatchClient.SetupSequence(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ThrowsAsync(new InvalidSequenceTokenException("ExpectedSequenceToken"))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = "NewSequenceToken" });

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        var act = async () => await service.LogMessageAsync(jsonMessageData);

        await act.Should().NotThrowAsync<InvalidSequenceTokenException>();
        _mockCloudWatchClient.Verify(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Exactly(2));
    }

    [Fact]
    public async Task LogMessageAsync_ShouldUpdateSequenceToken_AfterSuccessfulPut()
    {
        var jsonMessageData = JsonConvert.SerializeObject(_messageData);

        _mockLogStreamService.Setup(m => m.CreateLogStreamAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("logStreamName");

        const string newSequenceToken = "NewSequenceToken";
        _mockCloudWatchClient.Setup(client => client.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default))
            .ReturnsAsync(new PutLogEventsResponse { NextSequenceToken = newSequenceToken });

        var service = new LoggingService(_mockCloudWatchClient.Object, _mockLogStreamService.Object);

        await service.LogMessageAsync(jsonMessageData);

        var sequenceTokenField = service.GetType().GetField("_sequenceToken", BindingFlags.NonPublic | BindingFlags.Instance);
        var sequenceTokenValue = sequenceTokenField?.GetValue(service);

        sequenceTokenValue.Should().Be(newSequenceToken);
    }
*/
}