using System.Globalization;
using System.Text.RegularExpressions;

namespace Net.Utils.CloudWatchHandler.Helpers;

public class LogStreamManager
{
    private static readonly Lazy<LogStreamManager> LazyInstance = new Lazy<LogStreamManager>(() => new LogStreamManager());

    public static LogStreamManager Instance => LazyInstance.Value;

    public string? CurrentLogStreamName { get; private set; }

    private LogStreamManager() { }

    public static void ResetInstanceForTesting()
    {
        // TODO: Reset LazyInstance, or provide another way to reset the singleton
    }

    public void UpdateLogStream(string? logStreamName)
    {
        CurrentLogStreamName = logStreamName;
    }

    public bool ShouldCreateNewStream()
    {
        if (string.IsNullOrEmpty(CurrentLogStreamName))
            return true;

        var match = Regex.Match(CurrentLogStreamName, @"\d{4}-\d{2}-\d{2}(-\d{2})?");
        if (!match.Success)
        {
            // Handle invalid date format in stream name
            return true;
        }

        var dateString = match.Value;
        DateTime lastStreamDate;

        try
        {
            lastStreamDate = DateTime.ParseExact(dateString, "yyyy-MM-dd-HH", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            try
            {
                lastStreamDate = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                return true; // If parsing fails, create a new stream
            }
        }

        // Your logic to determine if a new stream should be created
        return lastStreamDate.Date != DateTime.UtcNow.Date;
    }
}