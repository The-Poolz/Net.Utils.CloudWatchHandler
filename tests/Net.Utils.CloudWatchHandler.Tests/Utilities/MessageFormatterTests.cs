using System.Text.Json;
using FluentAssertions;
using Net.Utils.CloudWatchHandler.Utilities;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Utilities;

public class MessageFormatterTests
{
    [Fact]
    public void FormatExceptionMessage_ValidJson_ReturnsJsonData()
    {
        const string jsonData = "{\"Message\": \"TestMessage\"}";
        var result = MessageFormatter.FormatExceptionMessage(jsonData);
        result.Should().Be(jsonData);
    }

    [Fact]
    public void FormatExceptionMessage_MissingMessage_ThrowsArgumentException()
    {
        const string jsonData = "{\"notMessage\": \"TestMessage\"}";
        Action act = () => MessageFormatter.FormatExceptionMessage(jsonData);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FormatExceptionMessage_EmptyMessage_ThrowsArgumentNullException()
    {
        const string jsonData = "{\"Message\": \"\"}";
        Action act = () => MessageFormatter.FormatExceptionMessage(jsonData);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FormatExceptionMessage_NullMessage_ThrowsArgumentNullException()
    {
        const string jsonData = "{\"Message\": null}";
        Action act = () => MessageFormatter.FormatExceptionMessage(jsonData);
        act.Should().Throw<ArgumentNullException>();
    }
}