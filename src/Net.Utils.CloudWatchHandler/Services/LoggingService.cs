using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Net.Utils.CloudWatchHandler.Models;
using Newtonsoft.Json;

namespace Net.Utils.CloudWatchHandler.Services;

public class LoggingService
{
    private readonly IAmazonCloudWatchLogs _cloudWatchClient;
    private readonly string _logGroupName;
    private readonly ILogStreamService _logStreamService;
    private string? _sequenceToken;

    public LoggingService(IAmazonCloudWatchLogs cloudWatchClient, string logGroupName, ILogStreamService logStreamService)
    {
        _cloudWatchClient = cloudWatchClient ?? throw new ArgumentNullException(nameof(cloudWatchClient));
        _logGroupName = logGroupName ?? throw new ArgumentNullException(nameof(logGroupName));
        _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
    }

    public async Task LogMessageAsync(string? messageDataJson)
    {
        var messageData = JsonConvert.DeserializeObject<MessageData>(messageDataJson ?? string.Empty) ?? throw new System.InvalidOperationException();

        var logEvent = new InputLogEvent
        {
            Timestamp = DateTime.UtcNow,
            Message = JsonConvert.SerializeObject(messageData.MessageDetails)
        };

        var request = new PutLogEventsRequest
        {
            LogGroupName = _logGroupName,
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