## Net.Utils.CloudWatchHandler
CloudWatch Handler

[![CodeFactor](https://www.codefactor.io/repository/github/the-poolz/net.utils.cloudwatchhandler/badge)](https://www.codefactor.io/repository/github/the-poolz/net.utils.cloudwatchhandler)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=The-Poolz_Net.Utils.CloudWatchHandler&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=The-Poolz_Net.Utils.CloudWatchHandler)

`Net.Utils.CloudWatchHandler` is a .NET utility library designed to streamline interactions with AWS CloudWatch. It simplifies the process of creating and managing log streams and messages.

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
  - [Example with LoggingConfig](#example-with-loggingconfig)
- [Features](#features)
- [Contribution](#contribution)

## Installation

To install via NuGet:

```bash
Install-Package Net.Utils.CloudWatchHandler
```

## Usage
Below is a basic code example showcasing how to use the library's `LoggingService`.

```csharp
using Net.Utils.CloudWatchHandler.Services;

// Initialize the LoggingService
var loggingService = new LoggingService(cloudWatchClient, logStreamService);

// Log the message
await loggingService.LogMessageAsync(logConfiguration);
```
Comprehensive Example
The following example demonstrates how to implement a custom `LoggingConfig` class, and then use it to call `LogMessageAsync` from `LoggingService`.

```csharp
using Amazon.CloudWatchLogs;
using Microsoft.Extensions.Logging;

namespace AWSLambda
{
    public class LoggingConfig
    {
        // Singleton Instance
        private static readonly Lazy<LoggingConfig> _instance = new Lazy<LoggingConfig>(() => new LoggingConfig());
        public static LoggingConfig Instance => _instance.Value;

        public LoggingService LoggingService { get; }
        // ... Other properties ...

        private LoggingConfig()
        {
            IAmazonCloudWatchLogs client = new AmazonCloudWatchLogsClient();
            // Initialize LoggingService and other properties
        }

        public LogConfiguration CreateLogConfiguration(LogLevel errorLevel, string message, string applicationName)
        {
            // Create and return a new LogConfiguration object
        }
    }
}

// Usage
await LoggingConfig.Instance.LoggingService.LogMessageAsync(
    LoggingConfig.Instance.CreateLogConfiguration(LogLevel.Error, "Your Error Message", "Your Application Name")
);
```

## Features

- Easily create and manage AWS CloudWatch log streams.
- Optimized to create a new log stream only once per day.
- Appends messages to the existing log stream within the same day.
- Simplified logging service to send messages to CloudWatch.
- Customizable message formatter utility.

## Contribution

Contributions are welcome! Please create a pull request in the [GitHub repository](https://github.com/The-Poolz/Net.Utils.CloudWatchHandler/tree/master).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
