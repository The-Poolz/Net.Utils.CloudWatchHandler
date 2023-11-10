using Amazon.CloudWatchLogs;
using Net.Utils.CloudWatchHandler.Models;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.AwsCloudWatch;

namespace Net.Utils.CloudWatchHandler.Services;

public static class LoggerFactory
{
    public static Logger CreateLogger(LogConfig logConfig, IAmazonCloudWatchLogs client)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(logConfig.RestrictedToMinimumLevel)
            .Enrich.FromLogContext()
            .WriteTo.AmazonCloudWatch(
                logGroup: logConfig.LogGroup,
                logStreamPrefix: logConfig.LogStreamNamePrefix,
                restrictedToMinimumLevel: logConfig.RestrictedToMinimumLevel,
                appendHostName: logConfig.AppendHostName,
                appendUniqueInstanceGuid: logConfig.AppendUniqueInstanceGuid,
                cloudWatchClient: client);

        return loggerConfiguration.CreateLogger();
    }
}