using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Moq;
using Net.Utils.CloudWatchHandler.Models;
using Net.Utils.CloudWatchHandler.Services;
using Newtonsoft.Json;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LoggingServiceTests
{
    [Fact]
    public async Task LogMessageAsync_ShouldCall_CreateLogStreamAsync()
    {
        var mockCloudWatchClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamService = new Mock<LogStreamService>(MockBehavior.Strict, mockCloudWatchClient.Object, "logGroupName");
        var service = new LoggingService(mockCloudWatchClient.Object, "logGroupName", mockLogStreamService.Object);

        var messageData = new MessageData
        {
            Prefix = "prefix",
            DateTimeFormat = "daily"
        };

        var serializedMessageData = JsonConvert.SerializeObject(messageData);

        mockLogStreamService.Setup(m => m.CreateLogStreamAsync("prefix", "daily")).ReturnsAsync("someLogStream").Verifiable();

        await service.LogMessageAsync(serializedMessageData);

        mockLogStreamService.Verify();
    }

    [Fact]
    public async Task LogMessageAsync_ShouldCall_PutLogEventsAsync()
    {
        var mockCloudWatchClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamService = new Mock<LogStreamService>(MockBehavior.Strict, mockCloudWatchClient.Object, "logGroupName");
        var service = new LoggingService(mockCloudWatchClient.Object, "logGroupName", mockLogStreamService.Object);

        var messageData = new MessageData
        {
            Prefix = "prefix",
            DateTimeFormat = "daily"
        };

        var serializedMessageData = JsonConvert.SerializeObject(messageData);

        mockLogStreamService.Setup(m => m.CreateLogStreamAsync("prefix", "daily")).ReturnsAsync("someLogStream");
        mockCloudWatchClient.Setup(m => m.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default)).ReturnsAsync(new PutLogEventsResponse());

        await service.LogMessageAsync(serializedMessageData);

        mockCloudWatchClient.Verify(m => m.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), default), Times.Once);
    }

}