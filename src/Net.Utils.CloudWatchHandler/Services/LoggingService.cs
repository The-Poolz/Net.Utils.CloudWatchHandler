using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Net.Utils.CloudWatchHandler.Models;
using Newtonsoft.Json;
using InvalidOperationException = System.InvalidOperationException;

namespace Net.Utils.CloudWatchHandler.Services
{
    public class LoggingService
    {
        private readonly IAmazonCloudWatchLogs _cloudWatchClient;
        private readonly LogStreamService _logStreamService;
        private string? _sequenceToken;
        private const int MaxRetries = 3;

        public LoggingService(IAmazonCloudWatchLogs cloudWatchClient, LogStreamService logStreamService)
        {
            _cloudWatchClient = cloudWatchClient ?? throw new ArgumentNullException(nameof(cloudWatchClient));
            _logStreamService = logStreamService ?? throw new ArgumentNullException(nameof(logStreamService));
        }

        public virtual async Task LogMessageAsync(LogConfiguration logConfiguration)
        {
            var logEvent = new InputLogEvent
            {
                Timestamp = DateTime.UtcNow,
                Message = JsonConvert.SerializeObject(logConfiguration.Details)
            };

            var request = new PutLogEventsRequest
            {
                LogGroupName = logConfiguration.LogGroupName,
                LogStreamName = await _logStreamService.CreateLogStreamAsync(logConfiguration.LogGroupName ?? throw new InvalidOperationException(), logConfiguration.LogStreamName ?? throw new InvalidOperationException()),
                LogEvents = new List<InputLogEvent> { logEvent },
                SequenceToken = _sequenceToken
            };

            var retryCount = 0;
            while (retryCount < MaxRetries)
            {
                try
                {
                    var response = await _cloudWatchClient.PutLogEventsAsync(request);
                    _sequenceToken = response.NextSequenceToken;
                    break;
                }
                catch (InvalidSequenceTokenException ex)
                {
                    _sequenceToken = ex.ExpectedSequenceToken;
                    retryCount++;
                }
            }
        }
    }
}
