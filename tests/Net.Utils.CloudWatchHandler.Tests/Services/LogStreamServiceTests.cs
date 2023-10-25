using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Moq;
using Net.Utils.CloudWatchHandler.Helpers;
using Net.Utils.CloudWatchHandler.Services;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LogStreamServiceTests
{
    private const string LogGroupName = "logGroupName";
    private const int streamCreationIntervalInMinutes = 5;

    private readonly Mock<IAmazonCloudWatchLogs> _mockClient;
    private readonly Mock<LogStreamManager> _mockLogStreamManager;
    private readonly LogStreamService _logStreamService;

    public LogStreamServiceTests()
    {
        _mockClient = new Mock<IAmazonCloudWatchLogs>();
        _mockLogStreamManager = new Mock<LogStreamManager>();
        _logStreamService = new LogStreamService(_mockClient.Object, _mockLogStreamManager.Object);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCall_ShouldCreateNewStream()
    {
        await _logStreamService.CreateLogStreamAsync("prefix", streamCreationIntervalInMinutes, LogGroupName);

        _mockLogStreamManager.Verify(manager => manager.ShouldCreateNewStream(streamCreationIntervalInMinutes), Times.Once);
    }
    /*
    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateNewStream_WhenRequired()
    {
        _mockLogStreamManager.Setup(m => m.ShouldCreateNewStream(DateTimeFormat)).Returns(true);
        var service = new LogStreamService(_mockClient.Object, LogGroupName, _mockLogStreamManager.Object);

        var result = await service.CreateLogStreamAsync("prefix", DateTimeFormat);

        result.Should().NotBeNullOrEmpty();
        _mockClient.Verify(client => client.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12-23-yy")]
    public async Task CreateLogStreamAsync_ShouldReturnNull_IfDateTimeFormatIsNullOrEmpty(string dateTimeFormat)
    {
        _mockLogStreamManager.Setup(m => m.ShouldCreateNewStream(DateTimeFormat)).Returns(true);

        var result = await _logStreamService.CreateLogStreamAsync("prefix", dateTimeFormat);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateNewStream_WhenDateTimeFormat_Wrong()
    {
        _mockLogStreamManager.Setup(m => m.ShouldCreateNewStream(DateTimeFormat)).Returns(true);
        var service = new LogStreamService(_mockClient.Object, LogGroupName, _mockLogStreamManager.Object);

        var result = await service.CreateLogStreamAsync("prefix", string.Empty);

        result.Should().BeNull();
        _mockClient.Verify(client => client.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default), Times.Never);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldReturnCurrentLogStreamData_IfShouldNotCreateNewStream()
    {
        const string currentLogStreamData = "currentLogStreamData";
        _mockLogStreamManager.Setup(x => x.ShouldCreateNewStream(It.IsAny<string>())).Returns(false);
        _mockLogStreamManager.Setup(x => x.CurrentLogStreamData).Returns(currentLogStreamData);

        var result = await _logStreamService.CreateLogStreamAsync("prefix", "dateTimeFormat");

        result.Should().Be(currentLogStreamData);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateNewLogStream_IfShouldCreateNewStream()
    {
        const string newLogStreamName = "prefix-2023-10-24";
        _mockLogStreamManager.Setup(x => x.ShouldCreateNewStream(It.IsAny<string>())).Returns(true);
        _mockLogStreamManager.Setup(x => x.UpdateLogStream(It.IsAny<string>()));

        var result = await _logStreamService.CreateLogStreamAsync("prefix", DateTimeFormat);

        result.Should().Be(LogStreamService.GenerateLogStreamName("prefix", DateTimeFormat));
        _mockLogStreamManager.Verify(x => x.UpdateLogStream(It.Is<string>(s => s == newLogStreamName)), Times.Once);
    }
    */
}