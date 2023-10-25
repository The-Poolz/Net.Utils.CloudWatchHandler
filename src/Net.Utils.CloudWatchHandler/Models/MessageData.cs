namespace Net.Utils.CloudWatchHandler.Models;

public class MessageData
{
    public MessageData(string prefix, string dateTimeFormat, string logGroupName, MessageDetails messageDetails)
    {
        Prefix = prefix;
        DateTimeFormat = dateTimeFormat;
        LogGroupName = logGroupName;
        MessageDetails = messageDetails;
    }

    public string Prefix { get; set; }
    public string DateTimeFormat { get; set; }
    public string LogGroupName { get; set; }
    public MessageDetails MessageDetails { get; set; }
}