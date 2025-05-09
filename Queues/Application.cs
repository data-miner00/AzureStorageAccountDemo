namespace Queues;

using Azure.Storage.Queues;
using System;
using System.Text.Json;
using System.Threading.Tasks;

internal sealed class Application
{
    private readonly QueueServiceClient client;
    private readonly string queueName;

    public Application(QueueServiceClient client, string queueName)
    {
        this.client = client;
        this.queueName = queueName;
    }

    public async Task<QueueClient> CreateQueueAsync()
    {
        var response = await client.CreateQueueAsync(queueName).ConfigureAwait(false);

        if (response.GetRawResponse().Status != 201)
        {
            throw new InvalidOperationException($"Failed to create queue '{queueName}'.");
        }

        Console.WriteLine($"Queue '{queueName}' created.");

        return response.Value;
    }

    public async Task<QueueClient> GetQueueAsync(bool createIfNotExist = false)
    {
        var queue = client.GetQueueClient(queueName);

        if (!queue.Exists())
        {
            if (createIfNotExist)
            {
                var response = await queue.CreateIfNotExistsAsync().ConfigureAwait(false);

                if (response.Status != 201 && response.Status != 409)
                {
                    throw new InvalidOperationException($"Failed to get queue '{queueName}'.");
                }
            }
        }

        Console.WriteLine($"Queue '{queueName}' retrieved.");
        return queue;
    }

    public async Task SendMessageAsync(string message)
    {
        var queue = await GetQueueAsync().ConfigureAwait(false);
        var response = await queue.SendMessageAsync(message).ConfigureAwait(false);
        if (response.GetRawResponse().Status != 201)
        {
            throw new InvalidOperationException($"Failed to send message to queue '{queueName}'.");
        }
        Console.WriteLine($"Message sent to queue '{queueName}': {message}");
    }

    public async Task SendMessageAsync<T>(T message)
        where T : class
    {
        var queue = await GetQueueAsync().ConfigureAwait(false);
        var response = await queue.SendMessageAsync(JsonSerializer.Serialize(message)).ConfigureAwait(false);
        if (response.GetRawResponse().Status != 201)
        {
            throw new InvalidOperationException($"Failed to send message to queue '{queueName}'.");
        }
        Console.WriteLine($"Message sent to queue '{queueName}': {message}");
    }

    public async Task ConsumeAsync(int batchCount = 10)
    {
        var queue = await GetQueueAsync().ConfigureAwait(false);
        var messages = await queue.ReceiveMessagesAsync(batchCount).ConfigureAwait(false);
        if (messages.Value.Length > 0)
        {
            foreach (var message in messages.Value)
            {
                Console.WriteLine($"Received message: {message.MessageText}");
                await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt).ConfigureAwait(false);
            }
        }
        else
        {
            Console.WriteLine("No messages in the queue.");
        }
    }

    public async Task ListenAsync()
    {
        var queue = await GetQueueAsync().ConfigureAwait(false);

        while (true)
        {
            var message = await queue.ReceiveMessageAsync().ConfigureAwait(false);

            if (message.Value != null)
            {
                Console.WriteLine($"Received message: {message.Value.MessageText}");
                await queue.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt).ConfigureAwait(false);
            }

            await Task.Delay(1000).ConfigureAwait(false);
        }
    }

    public async Task PeekAsync()
    {
        var queue = await GetQueueAsync().ConfigureAwait(false);
        var peekedMessage = await queue.PeekMessageAsync().ConfigureAwait(false);
        if (peekedMessage.Value != null)
        {
            Console.WriteLine($"Peeked message: {peekedMessage.Value.MessageText}");
        }
        else
        {
            Console.WriteLine("No messages in the queue.");
        }
    }
}
