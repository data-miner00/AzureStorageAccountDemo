namespace Queues;

using Azure.Storage.Queues;
using Core;
using System;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// The sample application using queue.
/// </summary>
internal sealed class Application
{
    private readonly QueueServiceClient client;
    private readonly string queueName;

    /// <summary>
    /// Initializes a new instance of the <see cref="Application"/> class.
    /// </summary>
    /// <param name="client">The queue client.</param>
    /// <param name="queueName">The queue name.</param>
    public Application(QueueServiceClient client, string queueName)
    {
        this.client = Guard.ThrowIfNull(client);
        this.queueName = Guard.ThrowIfNullOrWhitespace(queueName);
    }

    /// <summary>
    /// Creates a queue.
    /// </summary>
    /// <returns>The queue client representing the created queue.</returns>
    /// <exception cref="InvalidOperationException">Throws when queue failed to create.</exception>
    public async Task<QueueClient> CreateQueueAsync()
    {
        var response = await this.client
            .CreateQueueAsync(this.queueName)
            .ConfigureAwait(false);

        if (response.GetRawResponse().Status != 201)
        {
            throw new InvalidOperationException($"Failed to create queue '{this.queueName}'.");
        }

        Console.WriteLine($"Queue '{this.queueName}' created.");

        return response.Value;
    }

    /// <summary>
    /// Retrieves the queue and optionally create it if not exist.
    /// </summary>
    /// <param name="createIfNotExist">A flag that indicated whether to create it on the fly.</param>
    /// <returns>The queue client representing the created queue.</returns>
    /// <exception cref="InvalidOperationException">Throws when queue failed to retrieve.</exception>
    public async Task<QueueClient> GetQueueAsync(bool createIfNotExist = false)
    {
        var queue = this.client.GetQueueClient(this.queueName);

        if (!await queue.ExistsAsync() && createIfNotExist)
        {
            var response = await queue.CreateIfNotExistsAsync().ConfigureAwait(false);

            if (response.Status != 201 && response.Status != 409)
            {
                throw new InvalidOperationException($"Failed to get queue '{this.queueName}'.");
            }
        }

        Console.WriteLine($"Queue '{this.queueName}' retrieved.");
        return queue;
    }

    /// <summary>
    /// Sends a message to the queue.
    /// </summary>
    /// <param name="message">The content.</param>
    /// <returns>The asynchronous task.</returns>
    /// <exception cref="InvalidOperationException">Throws when failed to send the message.</exception>
    public async Task SendMessageAsync(string message)
    {
        var queue = await this.GetQueueAsync().ConfigureAwait(false);
        var response = await queue.SendMessageAsync(message).ConfigureAwait(false);
        if (response.GetRawResponse().Status != 201)
        {
            throw new InvalidOperationException($"Failed to send message to queue '{this.queueName}'.");
        }

        Console.WriteLine($"Message sent to queue '{this.queueName}': {message}");
    }

    /// <summary>
    /// Sends a message to the queue.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="message">The content.</param>
    /// <returns>The asynchronous task.</returns>
    /// <exception cref="InvalidOperationException">Throws when failed to send the message.</exception>
    public async Task SendMessageAsync<T>(T message)
        where T : class
    {
        var queue = await this.GetQueueAsync().ConfigureAwait(false);
        var response = await queue.SendMessageAsync(JsonSerializer.Serialize(message)).ConfigureAwait(false);
        if (response.GetRawResponse().Status != 201)
        {
            throw new InvalidOperationException($"Failed to send message to queue '{this.queueName}'.");
        }

        Console.WriteLine($"Message sent to queue '{this.queueName}': {message}");
    }

    /// <summary>
    /// Consumes messages from the queue.
    /// </summary>
    /// <param name="batchCount">The max count to retrieve per batch.</param>
    /// <returns>The asynchronous task.</returns>
    public async Task ConsumeAsync(int batchCount = 10)
    {
        var queue = await this.GetQueueAsync().ConfigureAwait(false);
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

    /// <summary>
    /// Actively polling for new message from the queue.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous task.</returns>
    public async Task ListenAsync(CancellationToken cancellationToken)
    {
        var queue = await this.GetQueueAsync().ConfigureAwait(false);

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await queue.ReceiveMessageAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            if (message.Value != null)
            {
                Console.WriteLine($"Received message: {message.Value.MessageText}");
                await queue.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt).ConfigureAwait(false);
            }

            await Task.Delay(1000).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Peeks a message from the queue.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    public async Task PeekAsync()
    {
        var queue = await this.GetQueueAsync().ConfigureAwait(false);
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
