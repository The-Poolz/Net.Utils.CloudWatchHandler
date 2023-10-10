# Net.Utils.CloudWatchHandler
`Net.Utils.CloudWatchHandler` is a .NET utility library designed to streamline interactions with AWS CloudWatch, making it easier to create and manage log streams and messages.

## Installation

To install via NuGet:

```bash
Install-Package Net.Utils.CloudWatchHandler


### 3. **Usage**:
How to use your library/package. Include basic code examples.

```markdown
## Usage

```csharp
using Net.Utils.CloudWatchHandler;

// Initialize the service
var logStreamService = new LogStreamService(client, LogGroupName);
var loggingService = new LoggingService(client, LogGroupName, logStreamService);

// Create a new log stream and log a message
var LogStreamName = await logStreamService.CreateLogStreamAsync();
await loggingService.LogMessageAsync("Your Message", LogStreamName);


### 4. **Features**:
List the main features of your library/package.

```markdown
## Features

- Easily create and manage AWS CloudWatch log streams.
- Simplified logging service to send messages to CloudWatch.
- Customizable message formatter utility.

## Contribution

Contributions are welcome! Please create a pull request in the [GitHub repository](https://github.com/The-Poolz/Net.Utils.CloudWatchHandler/tree/master).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Authors
