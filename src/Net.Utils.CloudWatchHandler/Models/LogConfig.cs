using Serilog.Events;

namespace Net.Utils.CloudWatchHandler.Models
{
    /// <summary>
    /// Configuration settings for AWS CloudWatch logging.
    /// </summary>
    public class LogConfig
    {
        /// <summary>
        /// Gets or sets the name of the CloudWatch log group.
        /// </summary>
        public string LogGroup { get; set; } = null!;

        /// <summary>
        /// Gets or sets the prefix for the CloudWatch log stream names.
        /// </summary>
        public string? LogStreamNamePrefix { get; set; }

        /// <summary>
        /// Gets or sets the AWS region for the CloudWatch service.
        /// </summary>
        public string Region { get; set; } = null!;

        /// <summary>
        /// Gets or sets the minimum level of log events to output.
        /// </summary>
        public LogEventLevel RestrictedToMinimumLevel { get; set; } = LogEventLevel.Information;

        /// <summary>
        /// Gets or sets a value indicating whether the host name should be appended to the log stream name.
        /// </summary>
        public bool AppendHostName { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether a unique instance GUID should be appended to the log stream name.
        /// </summary>
        public bool AppendUniqueInstanceGuid { get; set; } = true;
    }
}

