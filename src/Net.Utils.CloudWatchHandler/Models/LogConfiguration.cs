using System.ComponentModel.DataAnnotations;

namespace Net.Utils.CloudWatchHandler.Models;

public class LogConfiguration
{
    public LogConfiguration(string? prefix, int streamCreationIntervalInMinutes, string? logGroupName,
        MessageDetails? details, string logStreamName)
    {
        Prefix = prefix;
        StreamCreationIntervalInMinutes = streamCreationIntervalInMinutes;
        LogGroupName = logGroupName;
        LogStreamName = logStreamName;
        Details = details;
    }

    [Required]
    [MaxLength(100)]
    public string? Prefix { get;  set; }

    [Range(1, 1440)]
    public int StreamCreationIntervalInMinutes { get;  set; }

    [Required]
    public string? LogGroupName { get; set; }

    [Required]
    public string? LogStreamName { get; set; }

    [Required]
    public MessageDetails? Details { get; set; }
}