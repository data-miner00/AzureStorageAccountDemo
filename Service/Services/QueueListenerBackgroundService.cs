namespace Service.Services;

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Service.Handler;
using Service.Options;

public sealed class QueueListenerBackgroundService : BackgroundService
{
    private readonly ILogger<QueueListenerBackgroundService> logger;
    private readonly QueueClient queueClient;
    private readonly WeatherHandler handler;
    private readonly TimeSpan pollingInterval;
    private readonly TimeSpan visibilityTimeout;
    private readonly int maxMessagesPerBatch;

    public QueueListenerBackgroundService(
        ILogger<QueueListenerBackgroundService> logger,
        QueueClient queueClient,
        QueueOption option,
        WeatherHandler handler)
    {
        this.logger = logger;
        this.queueClient = queueClient;
        this.handler = handler;
        this.pollingInterval = TimeSpan.FromSeconds(option.PollingIntervalInSeconds);
        this.visibilityTimeout = TimeSpan.FromSeconds(option.VisibilityTimeoutInSeconds);
        this.maxMessagesPerBatch = option.MaxMessagesPerBatch;
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
                    maxMessages: this.maxMessagesPerBatch,
                    visibilityTimeout: this.visibilityTimeout,
                    cancellationToken: stoppingToken);

                if (messages.Value.Length > 0)
                {
                    var tasks = messages.Value.Select(message => this.ProcessMessageAsync(message, stoppingToken));

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

            await this.handler.RouteAsync(message, cancellationToken);

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
