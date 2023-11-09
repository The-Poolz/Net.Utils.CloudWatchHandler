using Amazon.CloudWatchLogs;
using Moq;
using Net.Utils.CloudWatchHandler.Models;
using Net.Utils.CloudWatchHandler.Services;
using Xunit;
using FluentAssertions;
using Serilog;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class CloudWatchLogsTests
{
    private readonly LogConfig _logConfig = new()
    {
        LogGroup = "TestLogGroup",
        LogStreamNamePrefix = "TestLogStream",
        Region = "us-east-1",
        RestrictedToMinimumLevel = Serilog.Events.LogEventLevel.Information,
        AppendHostName = false,
        AppendUniqueInstanceGuid = true
    };

    [Fact]
    public void Create_Should_InitializeLogger()
    {
        var mockClient = new Mock<IAmazonCloudWatchLogs>();

        using var cloudWatchLogs = CloudWatchLogs.Create(_logConfig, mockClient.Object);

        cloudWatchLogs.Logger.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ShouldAssignLogger()
    {
        var mockLogger = new LoggerConfiguration().CreateLogger();
        var mockClient = new Mock<IAmazonCloudWatchLogs>().Object;

        var cloudWatchLogs = new CloudWatchLogs(mockLogger, mockClient);

        Assert.Equal(mockLogger, cloudWatchLogs.Logger);
    }

    [Fact]
    public void Constructor_ShouldAssignLoggerAndClient()
    {
        var mockLogger = new LoggerConfiguration().CreateLogger();
        var mockClient = new Mock<IAmazonCloudWatchLogs>().Object;

        var cloudWatchLogs = new CloudWatchLogs(mockLogger, mockClient);

        Assert.Equal(mockLogger, cloudWatchLogs.Logger);
        Assert.Equal(mockClient, cloudWatchLogs.Client);
    }
}