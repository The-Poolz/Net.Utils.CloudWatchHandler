using Net.Utils.CloudWatchHandler.Models;
using Xunit;
using FluentAssertions;
using Serilog.Events;

namespace Net.Utils.CloudWatchHandler.Tests.Models;

public class LogConfigTests
{
    [Fact]
    public void LogConfig_Should_HaveCorrectDefaultValues()
    {
        var logConfig = new LogConfig();

        logConfig.RestrictedToMinimumLevel.Should().Be(LogEventLevel.Information, "default restricted level should be Information");
        logConfig.AppendHostName.Should().BeFalse("default for appending host name should be false");
        logConfig.AppendUniqueInstanceGuid.Should().BeTrue("default for appending unique instance GUID should be true");
    }

    [Fact]
    public void LogConfig_Should_AllowPropertyModification()
    {
        var logConfig = new LogConfig
        {
            LogGroup = "MyLogGroup",
            LogStreamNamePrefix = "MyLogStreamPrefix",
            Region = "us-west-2",
            RestrictedToMinimumLevel = LogEventLevel.Error,
            AppendHostName = true,
            AppendUniqueInstanceGuid = false
        };

        logConfig.LogGroup.Should().Be("MyLogGroup", "LogGroup should be set to the provided value");
        logConfig.LogStreamNamePrefix.Should().Be("MyLogStreamPrefix", "LogStreamNamePrefix should be set to the provided value");
        logConfig.Region.Should().Be("us-west-2", "Region should be set to the provided value");
        logConfig.RestrictedToMinimumLevel.Should().Be(LogEventLevel.Error, "RestrictedToMinimumLevel should be set to the provided value");
        logConfig.AppendHostName.Should().BeTrue("AppendHostName should be set to the provided value");
        logConfig.AppendUniqueInstanceGuid.Should().BeFalse("AppendUniqueInstanceGuid should be set to the provided value");
    }
}