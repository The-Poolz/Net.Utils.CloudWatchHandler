using FluentAssertions;
using Microsoft.Extensions.Logging;
using Net.Utils.CloudWatchHandler.Models;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Models;

public class MessageDetailsTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        LogLevel? errorLevel = LogLevel.Error;
        const string message = "This is a test message";
        const string applicationName = "TestApplication";

        var messageDetails = new MessageDetails(errorLevel, message, applicationName);

        messageDetails.ErrorLevel.Should().Be(errorLevel);
        messageDetails.Message.Should().Be(message);
        messageDetails.ApplicationName.Should().Be(applicationName);
    }
}