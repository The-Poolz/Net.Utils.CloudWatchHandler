using Amazon.CloudWatchLogs.Model;
using Amazon.CloudWatchLogs;
using Net.Utils.CloudWatchHandler.Utilities;

namespace Net.Utils.CloudWatchHandler.Services;

public class LoggingService
{
    private readonly IAmazonCloudWatchLogs _cloudWatchClient;
    private readonly string _logGroupName;
    private readonly LogStreamService _logStreamService;
    private string? _sequenceToken;

    public LoggingService(IAmazonCloudWatchLogs cloudWatchClient, string logGroupName, LogStreamService logStreamService)
    {
        _cloudWatchClient = cloudWatchClient ?? throw new ArgumentNullException(nameof(cloudWatchClient));
        _logGroupName = logGroupName ?? throw new ArgumentNullException(nameof(logGroupName));
        _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
    }

    public async Task LogMessageAsync(string message)
    {
        var formattedMessage = MessageFormatter.FormatExceptionMessage(message);

        var logEvent = new InputLogEvent
        {
            Timestamp = DateTime.UtcNow,
            Message = formattedMessage
        };

        var request = new PutLogEventsRequest
        {
            LogGroupName = _logGroupName,
            LogStreamName = await _logStreamService.CreateLogStreamAsync(),
            LogEvents = new List<InputLogEvent> { logEvent },
            SequenceToken = _sequenceToken
        };

        try
        {
            var response = await _cloudWatchClient.PutLogEventsAsync(request);
            _sequenceToken = response.NextSequenceToken;
        }
        catch (InvalidSequenceTokenException ex)
        {
            _sequenceToken = ex.ExpectedSequenceToken;
            await LogMessageAsync(message); // Retry
        }
    }
}