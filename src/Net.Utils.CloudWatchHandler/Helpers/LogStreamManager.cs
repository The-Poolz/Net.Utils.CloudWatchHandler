namespace Net.Utils.CloudWatchHandler.Helpers;

public class LogStreamManager
{
    private static readonly Lazy<LogStreamManager> LazyInstance = new(() => new LogStreamManager());

    public static LogStreamManager Instance => LazyInstance.Value;

    public string? CurrentLogStreamName { get; private set; }
    public virtual DateTime LastStreamDate { get; private set; }

    public virtual void UpdateStreamData(string newLogStreamName)
    {
        CurrentLogStreamName = newLogStreamName;
        LastStreamDate = DateTime.UtcNow;
    }

    public virtual bool ShouldCreateNewStream(int streamCreationIntervalInMinutes)
    {
        if (string.IsNullOrEmpty(CurrentLogStreamName))
            return true;

        var timeSinceLastStreamInMinutes = (DateTime.UtcNow - LastStreamDate).TotalMinutes;

        return timeSinceLastStreamInMinutes >= streamCreationIntervalInMinutes;
    }
   
}