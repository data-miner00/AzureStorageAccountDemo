namespace Service.Options;

public class QueueOption
{
    public const string SectionName = "Queue";

    public int MaxMessagesPerBatch { get; set; }

    public int VisibilityTimeoutInSeconds { get; set; }

    public int PollingIntervalInSeconds { get; set; }
}
