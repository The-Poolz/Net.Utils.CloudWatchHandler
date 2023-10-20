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
    private readonly Mock<IAmazonCloudWatchLogs> _mockClient;
    private const string LogGroupName = "logGroupName";
    private readonly LogStreamService _service;

    public LogStreamServiceTests()
    {
        _mockClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamManager = new Mock<ILogStreamManager>();
        _service = new LogStreamService(_mockClient.Object, LogGroupName, mockLogStreamManager.Object);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCall_ShouldCreateNewStream()
    {
        var mockClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamManager = new Mock<ILogStreamManager>();
        var service = new LogStreamService(mockClient.Object, LogGroupName, mockLogStreamManager.Object);

        await service.CreateLogStreamAsync("prefix", "daily");

        mockLogStreamManager.Verify(manager => manager.ShouldCreateNewStream(), Times.Once);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateNewStream_WhenRequired()
    {
        var mockClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamManager = new Mock<ILogStreamManager>();
        mockLogStreamManager.Setup(m => m.ShouldCreateNewStream()).Returns(true);
        var service = new LogStreamService(mockClient.Object, LogGroupName, mockLogStreamManager.Object);

        var result = await service.CreateLogStreamAsync("prefix", "daily");

        result.Should().NotBeNullOrEmpty();
        mockClient.Verify(client => client.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldNotCreateNewStream_WhenNotRequired()
    {
        var mockClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamManager = new Mock<ILogStreamManager>();
        mockLogStreamManager.Setup(m => m.ShouldCreateNewStream()).Returns(false);
        var service = new LogStreamService(mockClient.Object, LogGroupName, mockLogStreamManager.Object);

        var result = await service.CreateLogStreamAsync("prefix", "daily");

        result.Should().BeNull();
        mockClient.Verify(client => client.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default), Times.Never);
    }

    [Fact]
    public async Task LogStreamExistsAsync_ShouldReturnTrue_WhenLogStreamExists()
    {
        _mockClient.Setup(client => client.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
            .ReturnsAsync(new DescribeLogStreamsResponse
            {
                LogStreams = new List<LogStream> { new LogStream() }
            });

        var result = await _service.LogStreamExistsAsync("existingLogStream");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task LogStreamExistsAsync_ShouldReturnFalse_WhenLogStreamDoesNotExist()
    {
        _mockClient.Setup(client => client.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
            .ReturnsAsync(new DescribeLogStreamsResponse
            {
                LogStreams = new List<LogStream>()
            });

        var result = await _service.LogStreamExistsAsync("nonExistingLogStream");

        result.Should().BeFalse();
    }

    [Fact]
    public void GenerateLogStreamName_ShouldReturnCorrectlyFormattedName_WhenGivenFrequency()
    {
        var result = LogStreamService.GenerateLogStreamName("prefix", "daily");

        result.Should().StartWith("prefix-");
        result.Should().EndWith(DateTime.UtcNow.ToString("yyyy-MM-dd"));
    }

    [Fact]
    public void GenerateLogStreamName_ShouldReturnCorrectlyFormattedName_WhenGivenHourlyFrequency()
    {
        var result = LogStreamService.GenerateLogStreamName("prefix", "hourly");

        result.Should().StartWith("prefix-");
        result.Should().EndWith(DateTime.UtcNow.ToString("yyyy-MM-dd-HH"));
    }
}