namespace Net.Utils.CloudWatchHandler.Models;

public class MessageData
{
    public MessageData(string prefix, int streamCreationIntervalInMinutes, string logGroupName, MessageDetails messageDetails)
    {
        Prefix = prefix;
        StreamCreationIntervalInMinutes = streamCreationIntervalInMinutes;
        LogGroupName = logGroupName;
        MessageDetails = messageDetails;
    }

    public string Prefix { get; set; }
    public int StreamCreationIntervalInMinutes { get; set; }
    public string LogGroupName { get; set; }
    public MessageDetails MessageDetails { get; set; }
}