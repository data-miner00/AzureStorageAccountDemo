namespace Service.Services;

/// <summary>
/// The background service that logs messages at regular intervals.
/// </summary>
public sealed class LogBackgroundService : BackgroundService
{
    private const string ServiceName = "LogBackgroundService";
    private const int LogIntervalSeconds = 20;

    private readonly ILogger<LogBackgroundService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogBackgroundService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LogBackgroundService(ILogger<LogBackgroundService> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return LogAndStop();

        async Task LogAndStop()
        {
            this.logger.LogInformation("Logging from {ServiceName} - StopAsync - {DateTime}", ServiceName, DateTime.Now);
            await base.StopAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        while (!stoppingToken.IsCancellationRequested)
        {
            this.logger.LogInformation("Logging from {ServiceName} - ExecuteAsync - {DateTime}", ServiceName, DateTime.Now);
            await Task.Delay(TimeSpan.FromSeconds(LogIntervalSeconds), stoppingToken);
        }
    }
}
