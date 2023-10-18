using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Net.Utils.CloudWatchHandler.Helpers;

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

    public virtual async Task<string?> CreateLogStreamAsync()
    {
        var manager = LogStreamManager.Instance;

        if (!manager.ShouldCreateNewStream())
            return manager.CurrentLogStreamName;

        var logStreamName = GenerateLogStreamName();
        await TryCreateLogStreamAsync(new CreateLogStreamRequest(_logGroupName, logStreamName));

        manager.UpdateLogStream(logStreamName);

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

    private static string? GenerateLogStreamName() => $"LambdaSetStream-{DateTime.UtcNow:yyyy-MM-dd}";

    private async Task TryCreateLogStreamAsync(CreateLogStreamRequest request)
    {
        await _client.CreateLogStreamAsync(request)
            .ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception?.InnerExceptions.FirstOrDefault() is { } exception && !(exception is ResourceAlreadyExistsException))
                    throw exception;
            });
    }
}