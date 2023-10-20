using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Moq;
using Net.Utils.CloudWatchHandler.Helpers;
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
        _mockClient = new Mock<IAmazonCloudWatchLogs>();
        _service = new LogStreamService(_mockClient.Object, TestLogGroupName);
        LogStreamManager.ResetInstanceForTesting();
    }




    [Fact]
    public async Task CreateLogStreamAsync_ShouldCall_ShouldCreateNewStream()
    {
        // Arrange
        var mockClient = new Mock<IAmazonCloudWatchLogs>();
        var mockLogStreamManager = new Mock<ILogStreamManager>();
        var service = new LogStreamService(mockClient.Object, TestLogGroupName, mockLogStreamManager.Object);

        await service.CreateLogStreamAsync("prefix", "daily");

        mockLogStreamManager.Verify(manager => manager.ShouldCreateNewStream(), Times.Once);
    }















    /*

    [Fact]
    public async Task CreateLogStreamAsync_ShouldReturnLogStreamName()
    { 
        var generatedStreamName = $"LambdaSetStream-{DateTime.UtcNow:yyyy-MM-dd}";

        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default))
            .Returns(Task.FromResult(new CreateLogStreamResponse()));

        var result = await _service.CreateLogStreamAsync();

        result.Should().Be(generatedStreamName);
    }

    [Fact]
    public async Task LogStreamExistsAsync_ShouldReturnTrueIfExists()
    {
        const string existingStreamName = "existingStream";
        _mockClient.Setup(x => x.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
            .Returns(Task.FromResult(new DescribeLogStreamsResponse
            {
                LogStreams = new List<LogStream> { new LogStream { LogStreamName = existingStreamName } }
            }));

        var result = await _service.LogStreamExistsAsync(existingStreamName);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task LogStreamExistsAsync_ShouldReturnFalseIfNotExists()
    {
        _mockClient.Setup(x => x.DescribeLogStreamsAsync(It.IsAny<DescribeLogStreamsRequest>(), default))
            .Returns(Task.FromResult(new DescribeLogStreamsResponse { LogStreams = new List<LogStream>() }));

        var result = await _service.LogStreamExistsAsync("nonexistentStream");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldThrowExceptionWhenClientThrows()
    {
        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default))
            .Throws(new Exception("CloudWatch Error"));

        var action = async () => { await _service.CreateLogStreamAsync(); };

        await action.Should().ThrowAsync<Exception>().WithMessage("CloudWatch Error");
    }

    [Fact]
    public async Task TryCreateLogStreamAsync_ShouldCallCreateLogStreamAsync()
    {
        var methodInfo = _service.GetType().GetMethod("TryCreateLogStreamAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var request = new CreateLogStreamRequest(TestLogGroupName, "someStreamName");
        object[] parameters = { request };

        if (methodInfo != null) await (Task)methodInfo.Invoke(_service, parameters)!;

        _mockClient.Verify(client => client.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default), Times.Once);
    }*/
}