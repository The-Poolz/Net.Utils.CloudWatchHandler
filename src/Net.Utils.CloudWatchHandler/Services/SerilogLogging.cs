using Amazon.CloudWatchLogs;
using Net.Utils.CloudWatchHandler.Models;
using Serilog.Core;

namespace Net.Utils.CloudWatchHandler.Services;

public static class SerilogLogging
{
    public static Logger CreateLogger(LogConfig logConfig, IAmazonCloudWatchLogs client)
    {
        var loggerConfiguration = new Serilog.LoggerConfiguration();
        return loggerConfiguration.CreateLogger();
    }
}