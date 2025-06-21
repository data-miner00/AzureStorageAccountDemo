namespace Service.Handler;

using Azure.Storage.Queues.Models;

/// <summary>
/// The base class for message handlers.
/// </summary>
/// <typeparam name="T">The type of event being handled.</typeparam>
public abstract class MessageHandler<T> : IMessageHandler
    where T : class
{
    /// <inheritdoc/>
    /// <exception cref="InvalidCastException">Throws when the message body fails to deserialize into <see cref="T"/>.</exception>
    public Task RouteAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        var eventData = message.Body.ToObjectFromJson<T>()
            ?? throw new InvalidCastException("Failed to deserialize message body to the expected type.");

        return this.HandleAsync(eventData, cancellationToken);
    }

    /// <summary>
    /// The handling method for the <typeparamref name="T"/> event.
    /// </summary>
    /// <param name="event">The event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task.</returns>
    protected abstract Task HandleAsync(T @event, CancellationToken cancellationToken);
}
