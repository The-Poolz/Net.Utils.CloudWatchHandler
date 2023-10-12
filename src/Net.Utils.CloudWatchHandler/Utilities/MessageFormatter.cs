using System.Text.Json;

namespace Net.Utils.CloudWatchHandler.Utilities;

public class MessageFormatter
{
    public static string FormatExceptionMessage(string jsonData)
    {
        using var doc = JsonDocument.Parse(jsonData);
        var root = doc.RootElement;

        if (root.TryGetProperty("message", out var messageElement))
        {
            var message = messageElement.GetString();

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            return jsonData;
        }
        else
        {
            throw new ArgumentException("The 'message' property is missing in the JSON data.");
        }
    }
}