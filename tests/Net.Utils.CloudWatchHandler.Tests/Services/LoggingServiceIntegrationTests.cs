using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Moq;
using Net.Utils.CloudWatchHandler.Services;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LoggingServiceIntegrationTests : IDisposable
{
    private readonly IAmazonCloudWatchLogs _cloudWatchClient;
    private const string TestLogGroupName = "ErrorDownloader";
    private readonly LogStreamService _logStreamService;
    private readonly LoggingService _loggingService;

    public LoggingServiceIntegrationTests()
    {
        var mockCloudWatchClient = new Mock<IAmazonCloudWatchLogs>();

        mockCloudWatchClient.Setup(x => x.PutLogEventsAsync(It.IsAny<PutLogEventsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutLogEventsResponse
            {
                NextSequenceToken = "123"
            });

        mockCloudWatchClient.Setup(x => x.GetLogEventsAsync(It.IsAny<GetLogEventsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetLogEventsResponse
            {
                Events = new List<OutputLogEvent>
                {
                    new()
                    {
                        Message = "IntegrationTestMessage"
                    }
                }
            });
        _cloudWatchClient = mockCloudWatchClient.Object;

        _logStreamService = new LogStreamService(_cloudWatchClient, TestLogGroupName);
        _loggingService = new LoggingService(_cloudWatchClient, TestLogGroupName, _logStreamService);
    }

    [Fact]
    public async Task LogMessageAsync_ShouldLogToCloudWatch()
    {
        const string testMessage = "IntegrationTestMessage";

        await _loggingService.LogMessageAsync(testMessage);

        // Get logs to verify
        var logEvents = await _cloudWatchClient.GetLogEventsAsync(new GetLogEventsRequest
        {
            LogGroupName = TestLogGroupName,
            LogStreamName = await _logStreamService.CreateLogStreamAsync() // This assumes the log stream has just been created by the LoggingService.
        });

        // Verify that the log message exists in the retrieved logs
        var exists = logEvents.Events.Any(e => e.Message.Contains(testMessage));
        Assert.True(exists);
    }

    // Clean up resources after tests
    public void Dispose()
    {
        // Consider cleaning up the resources you've created (e.g., deleting log streams, log groups, etc.)
        // This ensures that you don't keep accumulating costs after the tests are done.
    }
}