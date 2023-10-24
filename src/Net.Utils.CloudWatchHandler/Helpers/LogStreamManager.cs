using System.Globalization;
using System.Text.RegularExpressions;

namespace Net.Utils.CloudWatchHandler.Helpers;

public class LogStreamManager : ILogStreamManager
{
    private static readonly Lazy<LogStreamManager> LazyInstance = new(() => new LogStreamManager());

    public static LogStreamManager Instance => LazyInstance.Value;

    public string? CurrentLogStreamData { get; private set; }

    public void UpdateLogStream(string? fullLogStreamName)
    {
        var indexOfHyphen = fullLogStreamName?.IndexOf('-') ?? -1;
        CurrentLogStreamData = indexOfHyphen < 0 ? null : fullLogStreamName?[(indexOfHyphen + 1)..];
    }

    public bool ShouldCreateNewStream(string? dateTimeFormat)
    {
        if (string.IsNullOrEmpty(CurrentLogStreamData))
            return true;

        DateTime lastStreamDate;

        try
        {
            var regex = new Regex(BuildRegexPatternFromFormat(dateTimeFormat), RegexOptions.None, TimeSpan.FromMilliseconds(50));
            var match = regex.Match(CurrentLogStreamData);
            if (!match.Success) return true;

            var dateString = match.Value;
            lastStreamDate = DateTime.ParseExact(dateString, dateTimeFormat ?? throw new ArgumentNullException(nameof(dateTimeFormat)), CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return true;
        }

        var lastHash = GenerateHashForDateTime(lastStreamDate, dateTimeFormat);
        var currentHash = GenerateHashForDateTime(DateTime.UtcNow, dateTimeFormat);

        return !lastHash.Equals(currentHash);
    }

    public static string BuildRegexPatternFromFormat(string? dateTimeFormat)
    {
        return dateTimeFormat!
            .Replace("yyyy", @"\d{4}")
            .Replace("MM", @"\d{2}")
            .Replace("dd", @"\d{2}")
            .Replace("HH", @"\d{1,2}")
            .Replace("mm", @"\d{1,2}");
    }

    public static string GenerateHashForDateTime(DateTime dateTime, string? format) => dateTime.ToString(format);
}