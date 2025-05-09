namespace Functions;

using Azure.Storage.Queues.Models;
using Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class QueueTriggerFunction
{
    private readonly ILogger<QueueTriggerFunction> _logger;

    public QueueTriggerFunction(ILogger<QueueTriggerFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(QueueTriggerFunction))]
    public void Run([QueueTrigger(Constants.TestQueueName, Connection = "")] QueueMessage message)
    {
        _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
    }
}
