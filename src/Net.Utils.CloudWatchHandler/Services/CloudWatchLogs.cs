using Amazon.CloudWatchLogs;
using Net.Utils.CloudWatchHandler.Models;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;

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
        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(logConfig.RestrictedToMinimumLevel)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Async(a => a.AmazonCloudWatch(
                logGroup: logConfig.LogGroup,
                logStreamPrefix: logConfig.LogStreamNamePrefix,
                restrictedToMinimumLevel: logConfig.RestrictedToMinimumLevel,
                appendHostName: logConfig.AppendHostName,
                appendUniqueInstanceGuid: logConfig.AppendUniqueInstanceGuid,
                cloudWatchClient: client
            ))
            .CreateLogger();

        return new CloudWatchLogs(logger, client);
    }

    public void Dispose()
    {
        Logger.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (Logger is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();
        else
            Dispose();
        GC.SuppressFinalize(this);
    }
}