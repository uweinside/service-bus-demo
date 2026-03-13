# Service Bus Demo

Small .NET 8 console app that sends 10 messages to an Azure Service Bus queue and reads them back through a simple menu.

## Prerequisites

- .NET 8 SDK
- An Azure Service Bus namespace
- A queue that already exists in that namespace

## Configuration

Set the values directly in [Program.cs](Program.cs) before running:

- `DemoConnectionString`
- `DemoQueueName`

The app now uses source-only configuration for this demo and throws a startup error if either value is left empty.

## Run

```powershell
dotnet run
```

Menu options:

1. Send 10 messages
2. Read up to 10 messages
3. Exit