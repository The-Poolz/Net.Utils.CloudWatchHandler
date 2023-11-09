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
            .Filter.ByExcluding(logEvent =>
                logEvent.Properties.Any(prop => prop.Value.ToString().Contains("sensitive-data")))
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
            Logger.Dispose();
        GC.SuppressFinalize(this);
    }
}