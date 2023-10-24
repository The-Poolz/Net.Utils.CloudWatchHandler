using Net.Utils.CloudWatchHandler.Helpers;
using FluentAssertions;
using Xunit;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Net.Utils.CloudWatchHandler.Tests.Helpers;

public class LogStreamManagerTests
{
    private readonly LogStreamManager _logStreamManager = LogStreamManager.Instance;
    private const string DateTimeFormat = "yyyy-MM-dd-HH";

    [Fact]
    public void Instance_ShouldReturnSameInstance()
    {
        var instance1 = LogStreamManager.Instance;
        var instance2 = LogStreamManager.Instance;

        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void UpdateLogStream_ShouldUpdateCurrentLogStreamName()
    {
        const string logStreamName = "NewLogStream";

        LogStreamManager.Instance.UpdateLogStream(logStreamName);

        LogStreamManager.Instance.CurrentLogStreamData.Should().BeNull();
    }

    [Fact]
    public void ShouldCreateNewStream_GivenValidHourlyFormat_ReturnsFalse()
    {
        var logStreamName = $"MyPrefix-{DateTime.UtcNow:yyyy-MM-dd-HH}";
        LogStreamManager.Instance.UpdateLogStream(logStreamName);

        var result = LogStreamManager.Instance.ShouldCreateNewStream(DateTimeFormat);

        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldCreateNewStream_GivenValidDailyFormat_ReturnsFalse()
    {
        var logStreamName = $"MyPrefix-{DateTime.UtcNow:yyyy-MM-dd}";
        LogStreamManager.Instance.UpdateLogStream(logStreamName);

        var result = LogStreamManager.Instance.ShouldCreateNewStream("yyyy-MM-dd");

        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldCreateNewStream_GivenInvalidFormat_ReturnsTrue()
    {
        LogStreamManager.Instance.UpdateLogStream("InvalidFormat");

        var result = LogStreamManager.Instance.ShouldCreateNewStream(DateTimeFormat);

        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldCreateNewStream_GivenNullStreamName_ReturnsTrue()
    {
        LogStreamManager.Instance.UpdateLogStream(null);

        var result = LogStreamManager.Instance.ShouldCreateNewStream(DateTimeFormat);

        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldThrowRegexMatchTimeoutException_WhenTimeoutOccurs()
    {
        const string input = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa!";
        const string pattern = "(a+)+$";
        var timeout = TimeSpan.FromMilliseconds(1);

        Action act = () => Regex.Match(input, pattern, RegexOptions.None, timeout);

        act.Should().Throw<RegexMatchTimeoutException>();
    }

    [Fact]
    public void ShouldCompleteWithinTimeLimit_WhenPatternIsSimple()
    {
        const string input = "abcdef";
        const string pattern = "abc";
        var timeout = TimeSpan.FromSeconds(10);
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var match = Regex.Match(input, pattern, RegexOptions.None, timeout);
        stopwatch.Stop();

        match.Success.Should().BeTrue();
        stopwatch.Elapsed.Should().BeLessOrEqualTo(timeout);
    }

    [Theory]
    [InlineData(@"\d{4}-\d{2}-\d{2}(-\d{2})?", "2021-10-12", 10)]
    [InlineData(@"\d{4}-\d{2}-\d{2}(-\d{2})?", "2021-10-12-14", 10)]
    public void TestRegexPerformance(string pattern, string input, int timeout)
    {
        var regex = new Regex(pattern, RegexOptions.None, TimeSpan.FromMilliseconds(timeout));

        var stopwatch = Stopwatch.StartNew();
        var match = regex.Match(input);
        stopwatch.Stop();

        match.Success.Should().BeTrue();
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromMilliseconds(timeout));
    }

    [Theory]
    [InlineData("InvalidPrefix-Date-2022-11-11", "yyyy-MM-dd", true)]
    [InlineData("OnlyPrefix", "yyyy-MM-dd", true)]
    [InlineData("OnlyDate-2022-11-11", "yyyy-MM-dd-HH", true)]
    public void Should_Create_New_Stream_When_Invalid_Format(string logStreamName, string dateTimeFormat, bool expectedResult)
    {
        LogStreamManager.Instance.UpdateLogStream(logStreamName);

        var result = LogStreamManager.Instance.ShouldCreateNewStream(dateTimeFormat);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Should_Create_New_Stream_When_Stream_Name_Is_Null()
    {
        var logStreamManager = LogStreamManager.Instance;
        logStreamManager.UpdateLogStream(null);

        var result = logStreamManager.ShouldCreateNewStream("yyyy-MM-dd");

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("TRE-2023-10-23", "yyyy-MM-dd", true)]
    [InlineData("TRE-2023-10-24", "yyyy-MM-dd", false)]
    [InlineData("TRE-2023-10-22", "yyyy-MM-dd", true)]
    [InlineData("TRE-2023-10-23-16", "yyyy-MM-dd-HH", true)]
    [InlineData("TRE-2023-10-23-15", "yyyy-MM-dd-HH", true)]
    [InlineData("TRE-2023-10-24-07", "yyyy-MM-dd-HH", true)]
    public void ShouldCreateNewStream_ShouldReturnExpectedResult(string currentLogStreamName, string dateTimeFormat, bool expectedResult)
    {
        _logStreamManager.UpdateLogStream(currentLogStreamName);

        var shouldCreateNewStream = _logStreamManager.ShouldCreateNewStream(dateTimeFormat);

        shouldCreateNewStream.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("Prefix-2023-10-24", "yyyy-MM-dd", false)]
    [InlineData("Prefix-2023-10-23-12", "yyyy-MM-dd-HH", true)]
    [InlineData("Prefix-2023-10-23-12-30", "yyyy-MM-dd-HH-mm", true)]
    public void Should_Parse_DateTime_Correctly(string logStreamName, string format, bool expected)
    {
        var logStreamManager = LogStreamManager.Instance;
        logStreamManager.UpdateLogStream(logStreamName);

        var result = logStreamManager.ShouldCreateNewStream(format);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Prefix-2023-13-23", "yyyy-MM-dd")] // Invalid month
    [InlineData("Prefix-2023-10-32", "yyyy-MM-dd")] // Invalid day
    [InlineData("Prefix-2023-10-23-25", "yyyy-MM-dd-HH")] // Invalid hour
    public void Should_Create_New_Stream_When_Invalid_DateTime_Format(string logStreamName, string format)
    {
        var logStreamManager = LogStreamManager.Instance;
        logStreamManager.UpdateLogStream(logStreamName);

        var result = logStreamManager.ShouldCreateNewStream(format);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Prefix-2023-10-23", "yyyy/MM/dd")]
    [InlineData("Prefix-2023-10-23-12", "yyyy-MM-DD-HH")]
    public void Should_Create_New_Stream_When_Mismatched_Or_Invalid_Format(string logStreamName, string format)
    {
        var logStreamManager = LogStreamManager.Instance;
        logStreamManager.UpdateLogStream(logStreamName);

        var result = logStreamManager.ShouldCreateNewStream(format);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("yyyy-MM-dd", @"\d{4}-\d{2}-\d{2}")]
    [InlineData("yyyy-MM-dd-HH", @"\d{4}-\d{2}-\d{2}-\d{1,2}")]
    [InlineData("yyyy-MM", @"\d{4}-\d{2}")]
    public void BuildRegexPatternFromFormat_ShouldGenerateCorrectPattern(string inputFormat, string expectedPattern)
    {
        var result = LogStreamManager.BuildRegexPatternFromFormat(inputFormat);

        result.Should().Be(expectedPattern);
    }

    [Theory]
    [InlineData("2023-10-24", "yyyy-MM-dd")]
    [InlineData("2023-10-24 12", "yyyy-MM-dd HH")]
    [InlineData("2023-10", "yyyy-MM")]
    public void GenerateHashForDateTime_ShouldGenerateCorrectString(string expected, string format)
    {
        var dateTime = DateTime.ParseExact(expected, format, CultureInfo.InvariantCulture);

        var result = LogStreamManager.GenerateHashForDateTime(dateTime, format);

        result.Should().Be(expected);
    }
}