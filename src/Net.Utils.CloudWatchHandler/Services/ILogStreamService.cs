namespace Net.Utils.CloudWatchHandler.Services;

public interface ILogStreamService
{
    Task<string?> CreateLogStreamAsync(string? prefix, string? dateTimeFormat);
}