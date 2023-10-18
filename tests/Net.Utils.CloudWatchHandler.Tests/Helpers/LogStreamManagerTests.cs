using System.Diagnostics;
using Net.Utils.CloudWatchHandler.Helpers;
using FluentAssertions;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Helpers;

public class LogStreamManagerTests
{
    [Fact]
    public void SingletonInstance_ShouldReturnSameInstance()
    {
        var instance1 = LogStreamManager.Instance;
        var instance2 = LogStreamManager.Instance;

        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void UpdateLogStream_ShouldUpdateProperties()
    {
        var instance = LogStreamManager.Instance;
        const string streamName = "testStream";
        instance.UpdateLogStream(streamName);

        instance.CurrentLogStreamName.Should().Be(streamName);
        instance.LastLogStreamCreationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void ShouldCreateNewStream_ShouldReturnFalse_IfDateIsSame()
    {
        var instance = LogStreamManager.Instance;
        instance.UpdateLogStream("testStream");

        Debug.Assert(instance != null, nameof(instance) + " != null");
        var result = instance.ShouldCreateNewStream();
        result.Should().BeFalse();
    }

    [Fact]
    public void ResetInstanceForTesting_ShouldResetSingletonInstance()
    {
        LogStreamManager.Instance.UpdateLogStream("initialStream");
        LogStreamManager.Instance.CurrentLogStreamName.Should().Be("initialStream");

        LogStreamManager.ResetInstanceForTesting();

        LogStreamManager.Instance.CurrentLogStreamName.Should().BeNull();

        LogStreamManager.Instance.UpdateLogStream("newStream");
        LogStreamManager.Instance.CurrentLogStreamName.Should().Be("newStream");
    }
}