namespace Service.Attributes;

/// <summary>
/// The attribute for Queue handlers.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class HandlerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerAttribute"/> class.
    /// </summary>
    /// <param name="eventType">The type of event.</param>
    public HandlerAttribute(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);
        this.EventType = eventType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerAttribute"/> class.
    /// </summary>
    /// <param name="queueName">The queue name.</param>
    public HandlerAttribute(string queueName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);
        this.QueueName = queueName;
    }

    /// <summary>
    /// Gets the event type if set.
    /// </summary>
    public Type? EventType { get; }

    /// <summary>
    /// Gets the queue name if set.
    /// </summary>
    public string QueueName { get; } = string.Empty;
}
