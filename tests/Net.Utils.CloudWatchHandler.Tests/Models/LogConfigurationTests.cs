﻿using FluentAssertions;
using Net.Utils.CloudWatchHandler.Models;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Models;

public class LogConfigurationTests
{
    private const string ExpectedPrefix = "prefix";
    private const string ExpectedLogGroupName = "logGroupName";
    private readonly string _expectedLogStreamName = $"{ExpectedPrefix}-{DateTime.UtcNow:yyyy-MM-ddTHH-mm}";
    public MessageDetails ExpectedDetails = new(LogLevel.Error, "message", "LambdaSet");

    [Fact]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var logConfiguration = new LogConfiguration(ExpectedLogGroupName, ExpectedDetails, _expectedLogStreamName);

        logConfiguration.LogGroupName.Should().Be(ExpectedLogGroupName);
        logConfiguration.Details.Should().BeEquivalentTo(ExpectedDetails);
    }

    [Fact]
    public void Constructor_ShouldCreateNonNullObject()
    {
        var logConfiguration = new LogConfiguration(ExpectedLogGroupName, ExpectedDetails, _expectedLogStreamName);

        logConfiguration.Should().NotBeNull();
    }
}