namespace Net.Utils.CloudWatchHandler.Helpers;

public class LogStreamManager
{
    private static LogStreamManager? _instance;
    public static LogStreamManager Instance => _instance ??= new LogStreamManager();

    public string? CurrentLogStreamName { get; private set; }
    public DateTime LastLogStreamCreationDate { get; private set; }

    private LogStreamManager() { }

    public static void ResetInstanceForTesting()
    {
        _instance = null;
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