using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Moq;
using Net.Utils.CloudWatchHandler.Services;
using System.Reflection;
using Xunit;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LogStreamServiceTests
{
    private readonly Mock<IAmazonCloudWatchLogs> _mockClient;
    private readonly LogStreamService _service;
    private const string TestLogGroupName = "TestLogGroupName";

    public LogStreamServiceTests()
    {
        _mockClient = new Mock<Amazon.CloudWatchLogs.IAmazonCloudWatchLogs>();
        _service = new LogStreamService(_mockClient.Object, TestLogGroupName);
    }

    [Fact]
    public void ResetStaticVariables()
    {
        typeof(LogStreamService)
            .GetField("_currentLogStreamName", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, null);

        typeof(LogStreamService)
            .GetField("_lastLogStreamCreationDate", BindingFlags.Static | BindingFlags.NonPublic)
            ?.SetValue(null, DateTime.MinValue);
    }

    [Fact]
    public async Task LogStreamExistsAsync_ShouldReturnFalse_WhenLogStreamDoesNotExist()
    {
        _mockClient.Setup(x => x.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DescribeLogStreamsResponse { LogStreams = new List<LogStream>() });

        var result = await _service.LogStreamExistsAsync("NonExistingStream");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldThrowException_WhenCloudWatchClientThrowsException()
    {
        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("CloudWatch Error"));

        await Assert.ThrowsAsync<Exception>(() => _service.CreateLogStreamAsync());
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateLogStreamAndReturnName()
    {
        ResetStaticVariables();
        const string expectedLogStreamNamePrefix = "LambdaSetStream-";

        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateLogStreamResponse());

        var result = await _service.CreateLogStreamAsync();

        result.Should().StartWith(expectedLogStreamNamePrefix);
        _mockClient.Verify(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldNotCreateNewStream_WhenCalledOnTheSameDate()
    {
        const string expectedLogStreamNamePrefix = "LambdaSetStream-";

        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateLogStreamResponse());

        var result = await _service.CreateLogStreamAsync();

        result.Should().StartWith(expectedLogStreamNamePrefix);
        _mockClient.Verify(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()), Times.Never());
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
    [Fact]
    public async Task CreateLogStreamAsync_ShouldNotUseCustomLogStreamName()
    {
        const string customLogStreamName = "CustomLogStreamName";

        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateLogStreamResponse());

        var result = await _service.CreateLogStreamAsync();

        result.Should().NotBe(customLogStreamName);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldThrowExceptionForUnexpectedError()
    {
        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.InvalidOperationException());

        await Assert.ThrowsAsync<System.InvalidOperationException>(() => _service.CreateLogStreamAsync());
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldReuseExistingStreamName_WhenCalledTwiceOnTheSameDay()
    {
        ResetStaticVariables();

        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateLogStreamResponse());

        var firstLogStreamName = await _service.CreateLogStreamAsync();

        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateLogStreamResponse());

        var secondLogStreamName = await _service.CreateLogStreamAsync();

        secondLogStreamName.Should().Be(firstLogStreamName);
    }
}