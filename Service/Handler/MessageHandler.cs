namespace Service.Handler;

using Azure.Storage.Queues.Models;

public abstract class MessageHandler<T>
    where T : class
{
    public Task RouteAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        var eventData = message.Body.ToObjectFromJson<T>()
            ?? throw new InvalidCastException("Failed to deserialize message body to the expected type.");

        return this.HandleAsync(eventData, cancellationToken);
    }

    protected abstract Task HandleAsync(T @event, CancellationToken cancellationToken);
}
