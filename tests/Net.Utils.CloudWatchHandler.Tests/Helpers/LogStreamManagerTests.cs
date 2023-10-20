using Net.Utils.CloudWatchHandler.Helpers;
using FluentAssertions;
using Xunit;

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