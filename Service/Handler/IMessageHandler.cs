namespace Service.Handler;

using Azure.Storage.Queues.Models;

public interface IMessageHandler
{
    Task HandleAsync(object @event, CancellationToken cancellationToken);

    Task RouteAsync(QueueMessage message, CancellationToken cancellationToken);
}
