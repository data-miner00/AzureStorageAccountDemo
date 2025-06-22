namespace Service.Publishers;

using Azure.Storage.Queues;
using Core;
using System.Text.Json;

/// <summary>
/// The message publisher for sending messages to Azure Storage Queues.
/// </summary>
public sealed class MessagePublisher
{
    private readonly Dictionary<string, QueueClient> queues;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePublisher"/> class.
    /// </summary>
    /// <param name="queues">The queues dictionary.</param>
    public MessagePublisher(Dictionary<string, QueueClient> queues)
    {
        this.queues = Guard.ThrowIfNull(queues);
    }

    /// <summary>
    /// Publishes a message to the specified queue asynchronously.
    /// </summary>
    /// <param name="queueName">The queue name.</param>
    /// <param name="message">The message to be published.</param>
    /// <returns>The task.</returns>
    /// <exception cref="InvalidOperationException">The specified queue name is invalid.</exception>
    public async Task PublishAsync(string queueName, object message)
    {
        if (this.queues.TryGetValue(queueName, out var queueClient))
        {
            var messageContent = JsonSerializer.Serialize(message);
            await queueClient.SendMessageAsync(messageContent);
        }
        else
        {
            throw new InvalidOperationException($"Queue '{queueName}' not found.");
        }
    }
}
