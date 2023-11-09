## Net.Utils.CloudWatchHandler
CloudWatch Handler for .NET

[![CodeFactor](https://www.codefactor.io/repository/github/the-poolz/net.utils.cloudwatchhandler/badge)](https://www.codefactor.io/repository/github/the-poolz/net.utils.cloudwatchhandler)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=The-Poolz_Net.Utils.CloudWatchHandler&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=The-Poolz_Net.Utils.CloudWatchHandler)

`Net.Utils.CloudWatchHandler`  is a .NET utility library that simplifies the process of creating and managing AWS CloudWatch log streams and messages.

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Contribution](#contribution)
- [License](#License)


## Installation

To install via NuGet:

```bash
Install-Package Net.Utils.CloudWatchHandler
```

## Usage
Below is a basic example showcasing how to use the `CloudWatchLogs` service for logging messages.

```csharp
using Net.Utils.CloudWatchHandler.Services;
using Amazon.CloudWatchLogs;

// Configure AWS CloudWatch client
IAmazonCloudWatchLogs cloudWatchClient = new AmazonCloudWatchLogsClient();

// Configure logging
var logConfig = new LogConfig
{
    LogGroup = "YourLogGroup",
    LogStreamNamePrefix = "YourLogStreamPrefix",
    Region = "us-east-1",
    RestrictedToMinimumLevel = Serilog.Events.LogEventLevel.Information,
    AppendHostName = true,
    AppendUniqueInstanceGuid = true
};

// Create CloudWatchLogs instance
var cloudWatchLogs = CloudWatchLogs.Create(logConfig, cloudWatchClient);

// Log a message
cloudWatchLogs.Logger.Information("This is a test log message.");
```

## Features

- Simplifies creation and management of AWS CloudWatch log streams.
- Creates a new log stream once per day, optimizing resource usage.
- Appends messages to the same log stream within a single day.
- Provides a simplified interface for sending messages to CloudWatch.
- Includes a customizable message formatter.

## Contribution

Contributions are welcome! Please create a pull request in the [GitHub repository](https://github.com/The-Poolz/Net.Utils.CloudWatchHandler/tree/master).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
