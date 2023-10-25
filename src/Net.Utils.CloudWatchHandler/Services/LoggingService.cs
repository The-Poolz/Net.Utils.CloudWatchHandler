using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Net.Utils.CloudWatchHandler.Models;
using Newtonsoft.Json;

namespace Net.Utils.CloudWatchHandler.Services;

public class LoggingService
{
    private readonly IAmazonCloudWatchLogs _cloudWatchClient;
    private readonly ILogStreamService _logStreamService;
    private string? _sequenceToken;

    public LoggingService(IAmazonCloudWatchLogs cloudWatchClient, ILogStreamService logStreamService)
    {
        _cloudWatchClient = cloudWatchClient ?? throw new ArgumentNullException(nameof(cloudWatchClient));
        _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
    }

    public async Task LogMessageAsync(string? messageDataJson)
    {
        var messageData = JsonConvert.DeserializeObject<MessageData>(messageDataJson ?? string.Empty) ?? throw new System.InvalidOperationException();

        var logEvent = new InputLogEvent
        {
            Timestamp = DateTime.UtcNow,
            Message = messageData.MessageDetails != null ? JsonConvert.SerializeObject(messageData.MessageDetails) : throw new System.InvalidOperationException()
        };

        var request = new PutLogEventsRequest
        {
            LogGroupName = messageData.LogGroupName ?? throw new System.InvalidOperationException(),
            LogStreamName = await _logStreamService.CreateLogStreamAsync(messageData.Prefix, messageData.DateTimeFormat),
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
            await LogMessageAsync(messageDataJson);
        }
    }
}