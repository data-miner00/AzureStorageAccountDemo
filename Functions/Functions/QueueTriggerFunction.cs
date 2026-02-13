namespace Functions.Functions;

using Azure.Storage.Queues.Models;
using Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

/// <summary>
/// A function that triggers when there is a new message in the queue.
/// </summary>
public sealed class QueueTriggerFunction
{
    private readonly ILogger<QueueTriggerFunction> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueTriggerFunction"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public QueueTriggerFunction(ILogger<QueueTriggerFunction> logger)
    {
        this.logger = Guard.ThrowIfNull(logger);
    }

    /// <summary>
    /// The function entry point.
    /// </summary>
    /// <param name="message">The new message.</param>
    [Function(nameof(QueueTriggerFunction))]
    public void Run([QueueTrigger(Constants.TestQueueName, Connection = "")] QueueMessage message)
    {
        this.logger.LogInformation("Queue trigger function processed: {Content}", message.MessageText);
    }
}
