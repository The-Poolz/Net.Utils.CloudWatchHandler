using Net.Utils.CloudWatchHandler.Helpers;
using FluentAssertions;
using Xunit;
using Moq;

namespace Net.Utils.CloudWatchHandler.Tests.Helpers;

public class LogStreamManagerTests
{
    private readonly Mock<LogStreamManager> _logStreamManager = new() { CallBase = true };

    [Fact]
    public void Instance_ShouldReturnSameInstance()
    {
        var instance1 = LogStreamManager.Instance;
        var instance2 = LogStreamManager.Instance;

        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void ShouldCreateNewStream_ShouldReturnTrue_WhenCurrentLogStreamNameIsEmpty()
    {
        _logStreamManager.Object.UpdateStreamData("");

        var result = _logStreamManager.Object.ShouldCreateNewStream(5);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(-2, 5, false)]
    [InlineData(-10, 5, true)]
    [InlineData(-10, 50, false)]
    [InlineData(-100, 500, false)]
    [InlineData(-100, 5, true)]
    public void ShouldCreateNewStream_Test(int minutesSinceLastStream, int interval, bool expectedResult)
    {
        _logStreamManager.Object.UpdateStreamData("someStreamName");
        _logStreamManager.Setup(m => m.LastStreamDate).Returns(DateTime.UtcNow.AddMinutes(minutesSinceLastStream));

        var shouldCreateNewStream = _logStreamManager.Object.ShouldCreateNewStream(interval);

        shouldCreateNewStream.Should().Be(expectedResult);
    }

    [Fact]
    public void UpdateStreamData_ShouldUpdate_CurrentLogStreamName()
    {
        _logStreamManager.Object.UpdateStreamData("newStreamName");

        _logStreamManager.Object.CurrentLogStreamName.Should().StartWith("newStreamName");
    }

    [Fact]
    public void UpdateStreamData_ShouldUpdate_LastStreamDate()
    {
        _logStreamManager.Object.UpdateStreamData("newStreamName");
        var beforeUpdate = DateTime.UtcNow;

        _logStreamManager.Object.UpdateStreamData("streamName");
        var lastStreamDate = _logStreamManager.Object.LastStreamDate;

        lastStreamDate.Should().BeOnOrAfter(beforeUpdate);
        lastStreamDate.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void ShouldCalculateTimeSinceLastStreamInMinutes()
    {
        var lastStreamDate = new DateTime(2023, 10, 25, 12, 0, 0, DateTimeKind.Utc);
        var currentUtcDate = new DateTime(2023, 10, 25, 12, 30, 0, DateTimeKind.Utc);

        var timeSpan = currentUtcDate - lastStreamDate;
        var timeSinceLastStreamInMinutes = (int)Math.Round(timeSpan.TotalMinutes);

        timeSinceLastStreamInMinutes.Should().Be(30);
    }
}