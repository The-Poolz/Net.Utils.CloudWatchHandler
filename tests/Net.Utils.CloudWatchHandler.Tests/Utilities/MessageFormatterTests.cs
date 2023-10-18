using FluentAssertions;
using Net.Utils.CloudWatchHandler.Utilities;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Utilities;

public class MessageFormatterTests
{
    [Fact]
    public void FormatExceptionMessage_ShouldReturnSameString_WhenValidJsonDataProvided()
    {
        const string validJsonData = "{\"Message\": \"An exception occurred.\"}";
        var result = MessageFormatter.FormatExceptionMessage(validJsonData);
        result.Should().Be(validJsonData);
    }

    [Fact]
    public void FormatExceptionMessage_ShouldThrowArgumentNullException_WhenNullProvided()
    {
        string? nullJsonData = null;
        Action act = () => MessageFormatter.FormatExceptionMessage(nullJsonData);
        act.Should().Throw<ArgumentNullException>().WithMessage("The provided JSON data should not be null or empty. (Parameter 'jsonData')");
    }

    [Fact]
    public void FormatExceptionMessage_ShouldThrowArgumentNullException_WhenEmptyStringProvided()
    {
        const string? emptyJsonData = "";
        Action act = () => MessageFormatter.FormatExceptionMessage(emptyJsonData);
        act.Should().Throw<ArgumentNullException>().WithMessage("The provided JSON data should not be null or empty. (Parameter 'jsonData')");
    }
}