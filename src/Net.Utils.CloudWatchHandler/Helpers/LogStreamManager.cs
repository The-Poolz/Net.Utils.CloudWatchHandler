using System.Globalization;
using System.Text.RegularExpressions;

namespace Net.Utils.CloudWatchHandler.Helpers;

public class LogStreamManager : ILogStreamManager
{
    private static readonly Lazy<LogStreamManager> LazyInstance = new(() => new LogStreamManager());

    public static LogStreamManager Instance => LazyInstance.Value;

    public string? CurrentLogStreamName { get; private set; }

    private LogStreamManager() { }

    public void UpdateLogStream(string? logStreamName) => CurrentLogStreamName = logStreamName;

    public bool ShouldCreateNewStream()
    {
        if (string.IsNullOrEmpty(CurrentLogStreamName))
            return true;

        var match = Regex.Match(CurrentLogStreamName, @"\d{4}-\d{2}-\d{2}(-\d{2})?");
        if (!match.Success)
            return true;

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
                return true;
            }
        }

        return lastStreamDate.Date != DateTime.UtcNow.Date;
    }
}