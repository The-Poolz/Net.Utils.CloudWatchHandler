using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;

namespace Net.Utils.CloudWatchHandler.Services;

public class LogStreamService
{
    private readonly IAmazonCloudWatchLogs _client;
    private readonly string _logGroupName;

    public LogStreamService(IAmazonCloudWatchLogs client, string logGroupName)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logGroupName = logGroupName ?? throw new ArgumentNullException(nameof(logGroupName));
    }

    public virtual async Task<string> CreateLogStreamAsync(string? customLogStreamName = null!)
    {
        var logStreamName = customLogStreamName ?? GenerateLogStreamName();
        await TryCreateLogStreamAsync(new CreateLogStreamRequest(_logGroupName, logStreamName));
        return logStreamName;
    }

    public virtual async Task<bool> LogStreamExistsAsync(string logStreamName)
    {
        var response = await _client.DescribeLogStreamsAsync(new DescribeLogStreamsRequest
        {
            LogGroupName = _logGroupName,
            LogStreamNamePrefix = logStreamName
        });

        return response.LogStreams.Count > 0;
    }

    private static string GenerateLogStreamName() => $"LambdaSetStream-{DateTime.UtcNow:yyyy-MM-ddTHH-mm-ss}";

    private async Task TryCreateLogStreamAsync(CreateLogStreamRequest request)
    {
        await _client.CreateLogStreamAsync(request)
            .ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception?.InnerExceptions.FirstOrDefault() is { } exception and not ResourceAlreadyExistsException)
                    throw exception;
            });
    }
}