namespace Net.Utils.CloudWatchHandler.Helpers;

public interface ILogStreamManager
{
    string? CurrentLogStreamName { get; }
    bool ShouldCreateNewStream();
    void UpdateLogStream(string? logStreamName);
}