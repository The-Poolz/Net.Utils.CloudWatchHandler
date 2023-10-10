using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Moq;
using Net.Utils.CloudWatchHandler.Services;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LogStreamServiceTests
{
    private readonly Mock<IAmazonCloudWatchLogs> _mockClient;
    private readonly LogStreamService _service;
    private const string TestLogGroupName = "TestLogGroupName";
        
    public LogStreamServiceTests()
    {
        _mockClient = new Mock<IAmazonCloudWatchLogs>();
        _service = new LogStreamService(_mockClient.Object, TestLogGroupName);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateLogStreamAndReturnName()
    {
        const string expectedLogStreamNamePrefix = "LambdaSetStream-";
        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateLogStreamResponse());

        var result = await _service.CreateLogStreamAsync();

        result.Should().StartWith(expectedLogStreamNamePrefix);
        _mockClient.Verify(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogStreamExistsAsync_ShouldReturnTrueIfExists()
    {
        const string logStreamName = "testStream";
        _mockClient.Setup(x => x.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeLogStreamsResponse
            {
                LogStreams = new List<LogStream> { new LogStream { LogStreamName = logStreamName } }
            });

        var result = await _service.LogStreamExistsAsync(logStreamName);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task LogStreamExistsAsync_ShouldReturnFalseIfNotExists()
    {
        _mockClient.Setup(x =>
                x.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeLogStreamsResponse { LogStreams = new List<LogStream>() });

        var result = await _service.LogStreamExistsAsync("nonexistentStream");

        result.Should().BeFalse();
    }
}