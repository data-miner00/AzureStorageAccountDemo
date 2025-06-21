namespace Service.Handler;

using Azure.Storage.Queues.Models;

/// <summary>
/// The interface for message handlers.
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// Converts the message to the object type <typeparamref name="T"/> and pass it to handler function.
    /// </summary>
    /// <param name="message">The queue message object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    Task RouteAsync(QueueMessage message, CancellationToken cancellationToken);
}
