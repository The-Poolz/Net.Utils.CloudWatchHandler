using System;
using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Net.Utils.CloudWatchHandler.Models;
using Net.Utils.CloudWatchHandler.Utilities;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Utilities;

public class MessageFormatterTests
{
    [Fact]
    public void FormatExceptionMessage_ShouldThrowArgumentNullException_WhenMessageIsNull()
    {
        Action act = () => MessageFormatter.FormatExceptionMessage(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FormatExceptionMessage_ShouldThrowArgumentNullException_WhenMessageIsEmpty()
    {
        Action act = () => MessageFormatter.FormatExceptionMessage(new ExceptionData
        {
            ExceptionMessage = string.Empty,
        });

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FormatExceptionMessage_ShouldReturnValidJson_WhenGivenValidInput()
    {
        const string message = "Some message";
        const string exceptionType = "DownloaderException";
        const string applicationName = "LambdaSet";

        var exceptionData = new ExceptionData
        {
            ExceptionMessage = message,
            ExceptionType = exceptionType,
            ApplicationName = applicationName
        };

        var output = MessageFormatter.FormatExceptionMessage(exceptionData);

        var deserializedOutput = JsonSerializer.Deserialize<ExceptionData>(output);

        deserializedOutput.Should().NotBeNull();
        deserializedOutput?.ExceptionMessage.Should().Be(message);
        deserializedOutput?.ExceptionType.Should().Be(exceptionType);
        deserializedOutput?.ApplicationName.Should().Be(applicationName);
    }
}