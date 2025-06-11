namespace Service.Services;

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

public sealed class QueueListenerBackgroundService : BackgroundService
{
    private readonly ILogger<QueueListenerBackgroundService> logger;
    private readonly QueueClient queueClient;
    private readonly TimeSpan pollingInterval;

    public QueueListenerBackgroundService(
        ILogger<QueueListenerBackgroundService> logger,
        QueueClient queueClient)
    {
        this.logger = logger;
        this.queueClient = queueClient;
        this.pollingInterval = TimeSpan.FromSeconds(5);
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Queue Listener Background Service started");

        // Ensure queue exists
        await this.queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await this.queueClient.ReceiveMessagesAsync(
                    maxMessages: 32,
                    visibilityTimeout: TimeSpan.FromMinutes(1),
                    cancellationToken: stoppingToken);

                if (messages.Value.Length > 0)
                {
                    var tasks = new Task[messages.Value.Length];

                    for (int i = 0; i < messages.Value.Length; i++)
                    {
                        tasks[i] = this.ProcessMessageAsync(messages.Value[i], stoppingToken);
                    }

                    await Task.WhenAll(tasks);
                }
                else
                {
                    await Task.Delay(this.pollingInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                this.logger.LogInformation("Queue Listener has been stopped.");
                break;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in queue listener");
            }
        }

        this.logger.LogInformation("Queue Listener Background Service stopped");
    }

    private async Task ProcessMessageAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Processing message {MessageId}: {MessageText}", 
                message.MessageId, message.MessageText);

            // Delete message after successful processing
            await this.queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);

            this.logger.LogInformation("Successfully processed message {MessageId}", message.MessageId);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error processing message {MessageId}", message.MessageId);

            // Handle poison messages
            if (message.DequeueCount > 5)
            {
                this.logger.LogWarning("Message {MessageId} has been dequeued {DequeueCount} times. Treating as poison message.", 
                    message.MessageId, message.DequeueCount);

                await this.queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
            }
        }
    }
}
