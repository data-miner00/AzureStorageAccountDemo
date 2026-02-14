namespace Service.Options;

/// <summary>
/// The settings for queue.
/// </summary>
public class QueueOption
{
    /// <summary>
    /// The section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Queue";

    /// <summary>
    /// Gets or sets the max messages to receive per batch.
    /// </summary>
    public int MaxMessagesPerBatch { get; set; }

    /// <summary>
    /// Gets or sets the visibility timeout seconds.
    /// </summary>
    public int VisibilityTimeoutInSeconds { get; set; }

    /// <summary>
    /// Gets or sets the polling interval seconds.
    /// </summary>
    public int PollingIntervalInSeconds { get; set; }
}
