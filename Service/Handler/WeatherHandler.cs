namespace Service.Handler;

using Service.Attributes;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The message handler for <see cref="WeatherForecast"/> event.
/// </summary>
[Handler("weather")]
public class WeatherHandler : MessageHandler<WeatherForecast>
{
    private readonly ILogger<WeatherHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public WeatherHandler(ILogger<WeatherHandler> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    protected override Task HandleAsync(WeatherForecast @event, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Weather forecast received: {Date} - {TemperatureC}°C, {Summary}",
            @event.Date.ToShortDateString(),
            @event.TemperatureC,
            @event.Summary);

        return Task.CompletedTask;
    }
}
