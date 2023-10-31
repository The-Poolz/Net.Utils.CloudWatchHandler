using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Net.Utils.CloudWatchHandler.Models;

public class MessageDetails
{
    public MessageDetails(LogLevel? errorLevel, string message, string applicationName)
    {
        ErrorLevel = errorLevel;
        Message = message;
        ApplicationName = applicationName;
    }

    [EnumDataType(typeof(LogLevel))]
    public LogLevel? ErrorLevel { get; private set; }

    [Required]
    [MaxLength(500)]
    public string Message { get; private set; }

    [Required]
    public string ApplicationName { get; private set; }
}