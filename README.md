# Retry Simulator
 A project to simulate retry patterns with services using .NET Polly framework.
 
## Compatibility
Windows, Linux, Mac

## Benefits
1) Fires concurrent requests to server to simulate real-time retries. (Note - use it with extreme cautious)
2) Local server provided without needing to create a server to simulate retries and understand how it works.
2) Simulate retries with public server directly using simple JSON configuration file.
3) Provides tokenization(e.g. `correlation-id`) which able to correlate requests from client & server.
4) Writes results to project `/Result` directory in CSV format to visualize the data.

## Project Dependancies
1) Clone RetrySimulator GitHub project to your preferred environment.
2) Visual Studio 2019/Visual Studio Code
3) Download [.NET 5 Build SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
4) Ensure Visual Studio 2019 IDE have been configured with Nuget package source : https://api.nuget.org/v3/index.json

## Guide to setup & run project locally
### Running client simulator
1) Open preferred Windows Terminal & navigate to project(`<project path>/Client`)
2) Run `dotnet run` command
3) Choose based on the retry option below:
   - Constant Backoff (e.g. 1s, 1s, 1s)
   - Linear Backoff (e.g. 1s, 2s, 3s)
   - Exponential Backoff (e.g. 1s, 2s, 4s, 8s)
   - Exponential Jitter Backoff (e.g. 1.123s, 2.456s, 4.768s, 8.125s)
   - RetryAfter Backoff (e.g. 5s, 5s, 5s)
   - Aws Decorrelated Jitter Backoff (e.g. 1.123s, 2.456s, 2.768s, 4.125s)
   - Decorrelated Jitter Backoff V2 (e.g. 1.555s, 2.223s, 2.123s, 3.233s)
4) Configure `appSettings.json`:
 ```
 {
  "retry": {
    "minDelayIsMs": 1000,
    "maxDelayIsMs": 5000,
    "maxRetry": 50,
    "jitterStart": 1,
    "jitterEnd": 1000

  },
  "request": {
    "method": "Post",
    "requestUri": "https://localhost:5001/api/service",
    "headers": {
      "Authorization": "Bearer <token>"
    },
    "contentHeaders": {
      "Content-Type": "application/json"
    },
    "body": "{\"id\":\"{{ id }}\"}"
  }
}
 ```
### Running local server simulator
1) Open preferred Windows Terminal & navigate to project (`<project path>/Server`)
2) Run `dotnet run` command

## What's next?
1) To research on how to throttle/drop/lag incoming & outgoing traffic for Windows/Ubuntu.
2) Package as an .EXE format
