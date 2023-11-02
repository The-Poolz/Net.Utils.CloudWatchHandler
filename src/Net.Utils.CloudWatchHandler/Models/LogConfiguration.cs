using System.ComponentModel.DataAnnotations;

namespace Net.Utils.CloudWatchHandler.Models;

public class LogConfiguration
{
    public LogConfiguration(string logGroupName,
        MessageDetails? details, string logStreamName)
    {
        LogGroupName = logGroupName;
        LogStreamName = logStreamName;
        Details = details;
    }

    [Required]
    public string LogGroupName { get; set; }

    [Required]
    public string? LogStreamName { get; set; }

    [Required]
    public MessageDetails? Details { get; set; }
}