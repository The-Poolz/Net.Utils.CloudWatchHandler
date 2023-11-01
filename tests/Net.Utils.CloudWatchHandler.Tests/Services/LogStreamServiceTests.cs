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
    private const string? LogGroupName = "logGroupName";
    private const string CurrentLogStream = "currentLogStream";
    private const int StreamCreationIntervalInMinutes = 5;

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
    public async Task CreateLogStreamAsync_ShouldNotCreateNewStream_WhenNotNeeded()
    {
        _mockLogStreamManager.Setup(m => m.CurrentLogStreamName).Returns(CurrentLogStream);
        _mockLogStreamManager.Setup(m => m.ShouldCreateNewStream(It.IsAny<int>())).Returns(false);

        var logStreamService = new LogStreamService(_mockClient.Object, _mockLogStreamManager.Object);

        var result = await logStreamService.CreateLogStreamAsync("prefix", StreamCreationIntervalInMinutes, "logGroupName", "LambdaSet-2023/11/01/[$LATEST]fc");

        result.Should().BeEquivalentTo(CurrentLogStream);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldCreateNewStream_WhenNeeded()
    {
        _mockLogStreamManager.Setup(x => x.ShouldCreateNewStream(It.IsAny<int>())).Returns(true);
        var newLogStream = "prefix";
        _mockLogStreamManager.Setup(x => x.UpdateStreamData(It.IsAny<string>())).Callback<string>(name => newLogStream = name);

        var result = await _logStreamService.CreateLogStreamAsync("something else", StreamCreationIntervalInMinutes, LogGroupName, "LambdaSet-2023/11/01/[$LATEST]f");

        result.Should().Be(newLogStream);
    }

    [Fact]
    public async Task CreateLogStreamAsync_ShouldThrowException_WhenCreateLogStreamFails()
    {
        _mockLogStreamManager.Setup(x => x.ShouldCreateNewStream(It.IsAny<int>())).Returns(true);
        _mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default))
            .ThrowsAsync(new System.InvalidOperationException());

        Func<Task> act = async () => await _logStreamService.CreateLogStreamAsync("prefix", StreamCreationIntervalInMinutes, "logGroupName", "LambdaSet-2023/11/01/[$LATEST]fc");

        await act.Should().ThrowAsync<System.InvalidOperationException>();
    }

    [Fact]
    public async Task TryCreateLogStreamAsync_ShouldThrowException_WhenTaskIsFaulted()
    {
        var mockClient = new Mock<IAmazonCloudWatchLogs>();
        var mockException = new Exception("Test Exception");

        var logStreamManager = new LogStreamManager();
        var logStreamService = new LogStreamService(mockClient.Object, logStreamManager);

        mockClient.Setup(x => x.CreateLogStreamAsync(It.IsAny<CreateLogStreamRequest>(), default(CancellationToken)))
            .ThrowsAsync(mockException);

        var act = async () => await logStreamService.TryCreateLogStreamAsync(new CreateLogStreamRequest("logGroupName", "logStreamName"));

        await act.Should().ThrowAsync<Exception>().WithMessage("Test Exception");
    }
}