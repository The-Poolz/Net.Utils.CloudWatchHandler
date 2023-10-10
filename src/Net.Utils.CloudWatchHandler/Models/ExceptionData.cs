namespace Net.Utils.CloudWatchHandler.Models;

public class ExceptionData
{
    public LogLevel LogLevel { get; set; }
    public string? ExceptionType { get; set; }
    public string? ApplicationName { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? Time { get; set; }
}