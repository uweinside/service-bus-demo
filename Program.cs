using Azure.Messaging.ServiceBus;

const string DemoConnectionString = "";
const string DemoQueueName = "test-queue";

Console.WriteLine("Azure Service Bus queue demo");
Console.WriteLine();

ValidateConfiguration(DemoConnectionString, DemoQueueName);

await using var client = new ServiceBusClient(DemoConnectionString);

while (true)
{
	Console.WriteLine();
	Console.WriteLine("1. Send 10 messages");
	Console.WriteLine("2. Read up to 10 messages");
	Console.WriteLine("3. Exit");
	Console.Write("Select an option: ");

	var input = Console.ReadLine();

	switch (input)
	{
		case "1":
			await SendTenMessagesAsync(client, DemoQueueName);
			break;
		case "2":
			await ReadMessagesAsync(client, DemoQueueName);
			break;
		case "3":
			return;
		default:
			Console.WriteLine("Choose 1, 2, or 3.");
			break;
	}
}

static void ValidateConfiguration(string connectionString, string queueName)
{
	if (string.IsNullOrWhiteSpace(connectionString))
	{
		throw new InvalidOperationException("Set DemoConnectionString in Program.cs before running the demo.");
	}

	if (string.IsNullOrWhiteSpace(queueName))
	{
		throw new InvalidOperationException("Set DemoQueueName in Program.cs before running the demo.");
	}
}

static async Task SendTenMessagesAsync(ServiceBusClient client, string queueName)
{
	await using var sender = client.CreateSender(queueName);
	using var batch = await sender.CreateMessageBatchAsync();

	for (var i = 1; i <= 10; i++)
	{
		var message = new ServiceBusMessage($"Demo message {i} created at {DateTimeOffset.UtcNow:O}")
		{
			MessageId = Guid.NewGuid().ToString(),
			Subject = "service-bus-demo"
		};

		if (!batch.TryAddMessage(message))
		{
			throw new InvalidOperationException("The message batch became full before all 10 messages were added.");
		}
	}

	await sender.SendMessagesAsync(batch);
	Console.WriteLine("Sent 10 messages.");
}

static async Task ReadMessagesAsync(ServiceBusClient client, string queueName)
{
	await using var receiver = client.CreateReceiver(queueName);
	var messages = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));

	if (messages.Count == 0)
	{
		Console.WriteLine("No messages available.");
		return;
	}

	foreach (var message in messages)
	{
		Console.WriteLine($"[{message.MessageId}] {message.Body}");
		await receiver.CompleteMessageAsync(message);
	}

	Console.WriteLine($"Read and completed {messages.Count} message(s).");
}
