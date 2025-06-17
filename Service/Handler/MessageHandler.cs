namespace Service.Handler;

using Azure.Storage.Queues.Models;

public abstract class MessageHandler<T> : IMessageHandler
    where T : class
{
    public abstract Task HandleAsync(object @event, CancellationToken cancellationToken);

    public Task RouteAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        var eventData = message.Body.ToObjectFromJson<T>();

        if (eventData == null)
        {
            throw new InvalidCastException("Failed to deserialize message body to the expected type.");
        }

        return this.HandleAsync(eventData, cancellationToken);
    }
}
