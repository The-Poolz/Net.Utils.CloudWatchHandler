namespace Net.Utils.CloudWatchHandler.Utilities;

/// <summary>
/// Provides functionality to format exception messages.
/// </summary>
public class MessageFormatter
{
    /// <summary>
    /// Checks if the provided JSON data is null or empty.
    /// </summary>
    /// <param name="jsonData">The JSON data containing the exception message.</param>
    /// <returns>The same JSON data if it is not null or empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the JSON data is null, empty, or whitespace.</exception>
    public static string FormatExceptionMessage(string jsonData)
    {
        if (string.IsNullOrWhiteSpace(jsonData))
        {
            throw new ArgumentNullException(nameof(jsonData), "The provided JSON data should not be null or empty.");
        }

        return jsonData;
    }
}