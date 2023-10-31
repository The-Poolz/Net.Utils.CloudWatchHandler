using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Net.Utils.CloudWatchHandler.Models;
using Newtonsoft.Json;
using InvalidOperationException = System.InvalidOperationException;

namespace Net.Utils.CloudWatchHandler.Services;

public class LoggingService
{
    private readonly IAmazonCloudWatchLogs _cloudWatchClient;
    private readonly LogStreamService _logStreamService;
    private string? _sequenceToken;

    public LoggingService(IAmazonCloudWatchLogs cloudWatchClient, LogStreamService logStreamService)
    {
        _cloudWatchClient = cloudWatchClient ?? throw new ArgumentNullException(nameof(cloudWatchClient));
        _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
    }

    public virtual async Task LogMessageAsync(MessageData messageData)
    {
        var logEvent = new InputLogEvent
        {
            Timestamp = DateTime.UtcNow,
            Message = JsonConvert.SerializeObject(messageData.MessageDetails)
    };

        var request = new PutLogEventsRequest
        {
            LogGroupName = messageData.LogGroupName,
            LogStreamName = await _logStreamService.CreateLogStreamAsync(messageData.Prefix, messageData.StreamCreationIntervalInMinutes, messageData.LogGroupName),
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
            await LogMessageAsync(messageData);
        }
    }
}