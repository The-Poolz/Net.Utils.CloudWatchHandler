using Amazon.CloudWatchLogs;
using Net.Utils.CloudWatchHandler.Models;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;

namespace Net.Utils.CloudWatchHandler.Services;

public class CloudWatchLogs : IDisposable, IAsyncDisposable
{
    public Serilog.Core.Logger Logger { get; }

    private CloudWatchLogs(Serilog.Core.Logger logger)
    {
        Logger = logger;
    }

    public static CloudWatchLogs Create(LogConfig logConfig)
    {
        var client = new AmazonCloudWatchLogsClient(new AmazonCloudWatchLogsConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(logConfig.Region)
        });

        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(logConfig.RestrictedToMinimumLevel)
            .WriteTo.AmazonCloudWatch(
                logGroup: logConfig.LogGroup,
                logStreamPrefix: logConfig.LogStreamNamePrefix,
                restrictedToMinimumLevel: logConfig.RestrictedToMinimumLevel,
                appendHostName: logConfig.AppendHostName,
                appendUniqueInstanceGuid: logConfig.AppendUniqueInstanceGuid,
                cloudWatchClient: client)
            .CreateLogger();

        return new CloudWatchLogs(logger);
    }

    public void Dispose() => Logger?.Dispose();

    public async ValueTask DisposeAsync() => await Logger.DisposeAsync();
}