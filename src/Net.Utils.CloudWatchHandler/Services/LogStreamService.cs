using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;

namespace Net.Utils.CloudWatchHandler.Services;

public class LogStreamService
{
    private readonly IAmazonCloudWatchLogs _cloudWatchClient;

    public LogStreamService(IAmazonCloudWatchLogs client)
    {
        _cloudWatchClient = client ?? throw new ArgumentNullException(nameof(client));
    }

    public virtual async Task<string?> CreateLogStreamAsync(string logGroupName, string logStreamName)
    {
        await TryCreateLogStreamAsync(new CreateLogStreamRequest(logGroupName, logStreamName));

        return logStreamName;
    }

    public virtual async Task TryCreateLogStreamAsync(CreateLogStreamRequest request)
    {
        await _cloudWatchClient.CreateLogStreamAsync(request)
            .ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception?.InnerExceptions.FirstOrDefault() is { } exception && !(exception is ResourceAlreadyExistsException))
                    throw exception;
            });
    }
}