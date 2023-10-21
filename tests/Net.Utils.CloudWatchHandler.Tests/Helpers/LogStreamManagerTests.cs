using Net.Utils.CloudWatchHandler.Helpers;
using FluentAssertions;
using Xunit;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Net.Utils.CloudWatchHandler.Tests.Helpers;

public class LogStreamManagerTests
{
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

        LogStreamManager.Instance.CurrentLogStreamName.Should().Be(logStreamName);
    }

    [Fact]
    public void ShouldCreateNewStream_GivenValidHourlyFormat_ReturnsFalse()
    {
        var logStreamName = $"MyPrefix-{DateTime.UtcNow:yyyy-MM-dd-HH}";
        LogStreamManager.Instance.UpdateLogStream(logStreamName);

        var result = LogStreamManager.Instance.ShouldCreateNewStream();

        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldCreateNewStream_GivenValidDailyFormat_ReturnsFalse()
    {
        var logStreamName = $"MyPrefix-{DateTime.UtcNow:yyyy-MM-dd}";
        LogStreamManager.Instance.UpdateLogStream(logStreamName);

        var result = LogStreamManager.Instance.ShouldCreateNewStream();

        result.Should().BeFalse();
    }

    [Fact]
    public void ShouldCreateNewStream_GivenInvalidFormat_ReturnsTrue()
    {
        LogStreamManager.Instance.UpdateLogStream("InvalidFormat");

        var result = LogStreamManager.Instance.ShouldCreateNewStream();

        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldCreateNewStream_GivenNullStreamName_ReturnsTrue()
    {
        LogStreamManager.Instance.UpdateLogStream(null);

        var result = LogStreamManager.Instance.ShouldCreateNewStream();

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
    [InlineData("InvalidPrefix-Date-2022-11-11")]
    [InlineData("OnlyPrefix")]
    [InlineData("OnlyDate-2022-11-11")]
    public void ShouldCreateNewStream_InvalidLogStreamNameFormats_ShouldReturnTrue(string logStreamName)
    {
        LogStreamManager.Instance.UpdateLogStream(logStreamName);

        var result = LogStreamManager.Instance.ShouldCreateNewStream();

        result.Should().BeTrue();
    }
}