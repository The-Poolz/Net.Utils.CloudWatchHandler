# Net.Utils.CloudWatchHandler

[![CodeFactor](https://www.codefactor.io/repository/github/the-poolz/net.utils.cloudwatchhandler/badge)](https://www.codefactor.io/repository/github/the-poolz/net.utils.cloudwatchhandler)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=The-Poolz_Net.Utils.CloudWatchHandler&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=The-Poolz_Net.Utils.CloudWatchHandler)

`Net.Utils.CloudWatchHandler` is a .NET utility library designed to streamline interactions with AWS CloudWatch. It simplifies the process of creating and managing log streams and messages.

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Contribution](#contribution)
`Net.Utils.CloudWatchHandler` is a .NET utility library designed to streamline interactions with AWS CloudWatch, making it easier to create and manage log streams and messages.

## Installation

To install via NuGet:

```bash
Install-Package Net.Utils.CloudWatchHandler
```

## Usage
How to use your library/package. Include basic code examples.

```csharp
using Net.Utils.CloudWatchHandler;

// Initialize the service
var logStreamService = new LogStreamService(client, LogGroupName);
var loggingService = new LoggingService(client, LogGroupName, logStreamService);

// Create a new log stream and log a message
var LogStreamName = await logStreamService.CreateLogStreamAsync();
await loggingService.LogMessageAsync("Your Message", LogStreamName);
```

## Features
List the main features of your library/package.

- Easily create and manage AWS CloudWatch log streams.
- Simplified logging service to send messages to CloudWatch.
- Customizable message formatter utility.


## Contribution

Contributions are welcome! Please create a pull request in the [GitHub repository](https://github.com/The-Poolz/Net.Utils.CloudWatchHandler/tree/master).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
