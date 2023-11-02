using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using FluentAssertions;
using Moq;
using Net.Utils.CloudWatchHandler.Services;
using Xunit;
using InvalidOperationException = System.InvalidOperationException;

namespace Net.Utils.CloudWatchHandler.Tests.Services;

public class LogStreamServiceTests
{
    private const string? LogGroupName = "logGroupName";
    private const string CurrentLogStream = "currentLogStream";

    private readonly Mock<IAmazonCloudWatchLogs> _mockClient;
    private readonly LogStreamService _logStreamService;

    public LogStreamServiceTests()
    {
        _mockClient = new Mock<IAmazonCloudWatchLogs>();
        _logStreamService = new LogStreamService(_mockClient.Object);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateNewStream()
    {
        var result = await _logStreamService.CreateLogStreamAsync(LogGroupName, "LambdaSet-2023/11/01/[$LATEST]fc");

        result.Should().BeEquivalentTo("LambdaSet-2023/11/01/[$LATEST]fc");
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldThrowException_WhenCreateLogStreamFails()
    {
        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default))
            .ThrowsAsync(new InvalidOperationException());

        Func<Task> act = async () => await _logStreamService.CreateLogStreamAsync("logGroupName", "LambdaSet-2023/11/01/[$LATEST]fc");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task TryCreateLogStreamAsync_ShouldThrowException_WhenTaskIsFaulted()
    {
        var mockClient = new Mock<IAmazonCloudWatchLogs>();
        var mockException = new Exception("Test Exception");

        var logStreamService = new LogStreamService(mockClient.Object);

        mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default(CancellationToken)))
            .ThrowsAsync(mockException);

        var act = async () => await logStreamService.TryCreateLogStreamAsync(new CreateLogStreamRequest("logGroupName", "logStreamName"));

        await act.Should().ThrowAsync<Exception>().WithMessage("Test Exception");
    }
}