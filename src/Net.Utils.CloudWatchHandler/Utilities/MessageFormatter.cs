using Net.Utils.CloudWatchHandler.Models;
using System.Text.Json;

namespace Net.Utils.CloudWatchHandler.Utilities;

public class MessageFormatter
{
    public static string FormatExceptionMessage(ExceptionData exceptionData)
    {
        if (exceptionData == null)
        {
            throw new ArgumentNullException(nameof(exceptionData));
        }

        if (string.IsNullOrWhiteSpace(exceptionData.ExceptionMessage))
        {
            throw new ArgumentNullException(nameof(exceptionData.ExceptionMessage));
        }

        exceptionData.Time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        return JsonSerializer.Serialize(exceptionData);
    }
}