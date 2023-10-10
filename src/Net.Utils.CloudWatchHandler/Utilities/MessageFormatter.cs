using System.Text.Json;

namespace Net.Utils.CloudWatchHandler.Utilities;

public class MessageFormatter
{
    public static string FormatExceptionMessage(string message, string exceptionType = "DownloaderException", string applicationName = "LambdaSet")
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentNullException(nameof(message));
        }

        var exceptionData = new
        {
            ExceptionType = exceptionType,
            Application = applicationName,
            ExceptionMessage = message,
            Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        return JsonSerializer.Serialize(exceptionData);
    }
}