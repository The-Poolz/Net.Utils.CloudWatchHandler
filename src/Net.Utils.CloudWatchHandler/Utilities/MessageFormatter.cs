using System.Text.Json;

namespace Net.Utils.CloudWatchHandler.Utilities;

/// <summary>
/// Provides functionality to format exception messages.
/// </summary>
public class MessageFormatter
{
    /// <summary>
    /// Formats the exception message based on the provided JSON data.
    /// </summary>
    /// <param name="jsonData">The JSON data containing the exception message.</param>
    /// <returns>The formatted exception message.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the Message is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown if the 'Message' property is missing in the JSON data.</exception>
    public static string FormatExceptionMessage(string jsonData)
    {
        using var doc = JsonDocument.Parse(jsonData);
        var root = doc.RootElement;

        if (root.TryGetProperty("Message", out var messageElement))
        {
            var message = messageElement.GetString();

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            return jsonData;
        }
        else
        {
            throw new ArgumentException("The 'Message' property is missing in the JSON data.");
        }
    }
}