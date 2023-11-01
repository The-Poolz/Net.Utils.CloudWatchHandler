using System.ComponentModel.DataAnnotations;

namespace Net.Utils.CloudWatchHandler.Models;

public class LogConfiguration
{
    public LogConfiguration(string? prefix, int streamCreationIntervalInMinutes, string? logGroupName,
        MessageDetails? details)
    {
        Prefix = prefix;
        StreamCreationIntervalInMinutes = streamCreationIntervalInMinutes;
        LogGroupName = logGroupName;
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
    public MessageDetails? Details { get; set; }
}