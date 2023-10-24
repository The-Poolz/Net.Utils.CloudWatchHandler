using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Net.Utils.CloudWatchHandler.Helpers;

namespace Net.Utils.CloudWatchHandler.Services;

public class LogStreamService : ILogStreamService
{
    private readonly IAmazonCloudWatchLogs _client;
    private readonly string _logGroupName;
    private readonly ILogStreamManager _logStreamManager;

    public LogStreamService(IAmazonCloudWatchLogs client, string logGroupName, ILogStreamManager logStreamManager)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logGroupName = logGroupName ?? throw new ArgumentNullException(nameof(logGroupName));
        _logStreamManager = logStreamManager ?? throw new ArgumentNullException(nameof(logStreamManager));
    }

    public virtual async Task<string?> CreateLogStreamAsync(string? prefix, string? dateTimeFormat)
    {
        if (!_logStreamManager.ShouldCreateNewStream(dateTimeFormat))
            return _logStreamManager.CurrentLogStreamData;

        var fullLogStreamName = GenerateLogStreamName(prefix, dateTimeFormat);

        await TryCreateLogStreamAsync(new CreateLogStreamRequest(_logGroupName, fullLogStreamName));

        _logStreamManager.UpdateLogStream(fullLogStreamName);

        return fullLogStreamName;
    }

    public static string GenerateLogStreamName(string? prefix, string? dateTimeFormat) => $"{prefix}-{DateTime.UtcNow.ToString(dateTimeFormat)}";

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