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
    private readonly LogStreamService _logStreamzService;
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

        _loggingService = new LoggingService(_cloudWatchClient, TestLogGroupName, _logStreamzService);
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}