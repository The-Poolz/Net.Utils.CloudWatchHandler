namespace Net.Utils.CloudWatchHandler.Helpers;

public class LogStreamManager
{
    private static readonly Lazy<LogStreamManager> LazyInstance = new Lazy<LogStreamManager>(() => new LogStreamManager());

    public static LogStreamManager Instance => LazyInstance.Value;

    public string? CurrentLogStreamName { get; private set; }
    public DateTime LastLogStreamCreationDate { get; private set; }

    private LogStreamManager() { }

    public static void ResetInstanceForTesting()
    {
        // Our logic here. This could be complex with Lazy<T>
    }

    public void UpdateLogStream(string? logStreamName)
    {
        CurrentLogStreamName = logStreamName;
        LastLogStreamCreationDate = DateTime.UtcNow;
    }

    public bool ShouldCreateNewStream()
    {
        return LastLogStreamCreationDate.Date != DateTime.UtcNow.Date;
    }
}