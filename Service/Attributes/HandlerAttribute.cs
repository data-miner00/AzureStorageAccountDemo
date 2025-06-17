namespace Service.Attributes;

using Service.Handler;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class HandlerAttribute : Attribute
{
    public HandlerAttribute(Type eventType)
    {
        this.EventType = eventType;
    }

    public HandlerAttribute(string queueName)
    {
        this.QueueName = queueName;
    }

    public Type EventType { get; }

    public string QueueName { get; } = string.Empty;
}
