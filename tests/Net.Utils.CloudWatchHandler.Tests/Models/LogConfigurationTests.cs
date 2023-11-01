using FluentAssertions;
using Net.Utils.CloudWatchHandler.Models;
using Microsoft.Extensions.Logging;
using Xunit;
using Net.Utils.CloudWatchHandler.Services;

namespace Net.Utils.CloudWatchHandler.Tests.Models;

public class LogConfigurationTests
{
    [Fact]
    public void Constructor_ShouldInitializeAllProperties()
    {
        const string expectedPrefix = "prefix";
        const int expectedInterval = 10;
        const string expectedLogGroupName = "logGroupName";
        var expectedDetails = new MessageDetails(LogLevel.Error, "message", "LambdaSet");

        var logConfiguration = new LogConfiguration(expectedPrefix, expectedInterval, expectedLogGroupName, expectedDetails);

        logConfiguration.Prefix.Should().Be(expectedPrefix);
        logConfiguration.StreamCreationIntervalInMinutes.Should().Be(expectedInterval);
        logConfiguration.LogGroupName.Should().Be(expectedLogGroupName);
        logConfiguration.Details.Should().BeEquivalentTo(expectedDetails);
    }

    [Fact]
    public void Constructor_ShouldCreateNonNullObject()
    {
        const string expectedPrefix = "prefix";
        const int expectedInterval = 10;
        const string expectedLogGroupName = "logGroupName";
        var expectedDetails = new MessageDetails(LogLevel.Error, "message", "LambdaSet");

        var logConfiguration = new LogConfiguration(expectedPrefix, expectedInterval, expectedLogGroupName, expectedDetails);

        logConfiguration.Should().NotBeNull();
    }
}