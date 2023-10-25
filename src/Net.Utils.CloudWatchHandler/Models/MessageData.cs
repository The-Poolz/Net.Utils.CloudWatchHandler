namespace Net.Utils.CloudWatchHandler.Models;

public class MessageData
{
    public string? Prefix { get; set; }
    public string? DateTimeFormat { get; set; }
    public string? LogGroupName { get; set; }
    public MessageDetails? MessageDetails { get; set; }
}