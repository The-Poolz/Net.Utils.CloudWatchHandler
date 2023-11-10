using Amazon.CloudWatchLogs;
using Net.Utils.CloudWatchHandler.Models;

namespace Net.Utils.CloudWatchHandler.Services;

public class CloudWatchLogs : IDisposable, IAsyncDisposable
{
    public IAmazonCloudWatchLogs Client { get; }
    public Serilog.Core.Logger Logger { get; }

    public CloudWatchLogs(Serilog.Core.Logger logger, IAmazonCloudWatchLogs client)
    {
        Client = client;
        Logger = logger;
    }

    public static CloudWatchLogs Create(LogConfig logConfig, IAmazonCloudWatchLogs client)
    {
        var logger = LoggerFactory.CreateLogger(logConfig, client);
        return new CloudWatchLogs(logger, client);
    }

    public void Dispose()
    {
        Logger?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (Logger is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else
            Logger.Dispose();
        GC.SuppressFinalize(this);
    }
}