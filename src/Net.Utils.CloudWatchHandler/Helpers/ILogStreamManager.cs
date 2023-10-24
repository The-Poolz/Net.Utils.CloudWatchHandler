namespace Net.Utils.CloudWatchHandler.Helpers;

public interface ILogStreamManager
{
    string? CurrentLogStreamData { get; }
    bool ShouldCreateNewStream(string? dateTimeFormat);
    void UpdateLogStream(string? fullLogStreamName);
}