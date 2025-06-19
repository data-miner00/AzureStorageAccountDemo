namespace Service.Services;

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Service.Attributes;
using Service.Handler;
using Service.Options;
using System.Reflection;

/// <summary>
/// The background service that is listening to queue activities.
/// </summary>
public sealed class QueueListenerBackgroundService : BackgroundService
{
    private readonly ILogger<QueueListenerBackgroundService> logger;
    private readonly Dictionary<string, QueueClient> queues;
    private readonly TimeSpan pollingInterval;
    private readonly TimeSpan visibilityTimeout;
    private readonly int maxMessagesPerBatch;
    private readonly IServiceScope serviceScope;
    private readonly Dictionary<string, IMessageHandler> messageHandlers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueListenerBackgroundService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="option">The option for queues.</param>
    /// <param name="serviceScopeFactory">The service scope factory.</param>
    /// <param name="queues">The key value pair of queues clients.</param>
    public QueueListenerBackgroundService(
        ILogger<QueueListenerBackgroundService> logger,
        QueueOption option,
        IServiceScopeFactory serviceScopeFactory,
        Dictionary<string, QueueClient> queues)
    {
        this.logger = logger;
        this.queues = queues;
        this.pollingInterval = TimeSpan.FromSeconds(option.PollingIntervalInSeconds);
        this.visibilityTimeout = TimeSpan.FromSeconds(option.VisibilityTimeoutInSeconds);
        this.maxMessagesPerBatch = option.MaxMessagesPerBatch;
        this.serviceScope = serviceScopeFactory.CreateScope();
        this.InitializeMessageHandlers();
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Queue Listener Background Service started");

        List<Task> tasks = [];

        foreach (var (queueName, handler) in this.messageHandlers)
        {
            var queueClient = this.queues[queueName];
            await queueClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

            if (queueClient == null)
            {
                this.logger.LogError("Queue client for {QueueName} not found", queueName);
                continue;
            }

            tasks.Add(Spinlock());

            async Task Spinlock()
            {
                this.logger.LogInformation("Listening to queue: {QueueName}", queueName);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var messages = await queueClient.ReceiveMessagesAsync(
                            maxMessages: this.maxMessagesPerBatch,
                            visibilityTimeout: this.visibilityTimeout,
                            cancellationToken: stoppingToken);

                        if (messages.Value.Length > 0)
                        {
                            var tasks = messages.Value.Select(message => this.ProcessMessageAsync(message, handler, queueClient, stoppingToken));

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
            }
        }

        await Task.WhenAll(tasks);

        this.logger.LogInformation("Queue Listener Background Service stopped");
    }

    private static IEnumerable<Type> GetTypesWithHandlerAttribute()
    {
        var attributeType = typeof(HandlerAttribute);
        var assembly = Assembly.GetExecutingAssembly();

        var typesWithAttribute = assembly.GetTypes()
            .Where(t => t.IsClass && t.GetCustomAttributes(attributeType, inherit: false).Length != 0);

        return typesWithAttribute;
    }

    private async Task ProcessMessageAsync(QueueMessage message, IMessageHandler handler, QueueClient queueClient, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation(
                "Processing message {MessageId}: {MessageText}",
                message.MessageId,
                message.MessageText);

            await handler.RouteAsync(message, cancellationToken);

            // Delete message after successful processing
            await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);

            this.logger.LogInformation("Successfully processed message {MessageId}", message.MessageId);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error processing message {MessageId}", message.MessageId);

            // Handle poison messages
            if (message.DequeueCount > 5)
            {
                this.logger.LogWarning(
                    "Message {MessageId} has been dequeued {DequeueCount} times. Treating as poison message.",
                    message.MessageId,
                    message.DequeueCount);

                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
            }
        }
    }

    private void InitializeMessageHandlers()
    {
        var typesWithAttribute = GetTypesWithHandlerAttribute();

        foreach (var type in typesWithAttribute)
        {
            var handlerAttribute = type.GetCustomAttribute<HandlerAttribute>()
                ?? throw new InvalidOperationException("Attribute not found.");

            var queueName = handlerAttribute.QueueName;

            this.messageHandlers[queueName] = (IMessageHandler)this.serviceScope.ServiceProvider.GetService(type)!;
        }
    }
}
