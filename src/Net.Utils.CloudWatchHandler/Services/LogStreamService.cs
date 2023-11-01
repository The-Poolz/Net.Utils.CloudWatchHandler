using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Net.Utils.CloudWatchHandler.Helpers;

namespace Net.Utils.CloudWatchHandler.Services;

public class LogStreamService
{
    private readonly IAmazonCloudWatchLogs _client;
    private readonly LogStreamManager _logStreamManager;

    public LogStreamService(IAmazonCloudWatchLogs client, LogStreamManager logStreamManager)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logStreamManager = logStreamManager ?? throw new ArgumentNullException(nameof(logStreamManager));
    }

    public virtual async Task<string?> CreateLogStreamAsync(string? prefix, int streamCreationIntervalInMinutes, string? logGroupName)
    {
        if (!_logStreamManager.ShouldCreateNewStream(streamCreationIntervalInMinutes))
            return _logStreamManager.CurrentLogStreamName;

        var logStreamName = GenerateLogStreamName(prefix);

        await TryCreateLogStreamAsync(new CreateLogStreamRequest(logGroupName, logStreamName));

        _logStreamManager.UpdateStreamData(logStreamName);

        return logStreamName;
    }

    public static string GenerateLogStreamName(string? prefix)
        => $"{prefix}-{DateTime.UtcNow:yyyy-MM-ddTHH-mm}";

    public virtual async Task TryCreateLogStreamAsync(CreateLogStreamRequest request)
    {
        await _client.CreateLogStreamAsync(request)
            .ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception?.InnerExceptions.FirstOrDefault() is { } exception && !(exception is ResourceAlreadyExistsException))
                    throw exception;
            });
    }
}