namespace Service.Handler;

using System.Threading;
using System.Threading.Tasks;

public class WeatherHandler : MessageHandler<WeatherForecast>
{
    private readonly ILogger<WeatherHandler> logger;

    public WeatherHandler(ILogger<WeatherHandler> logger)
    {
        this.logger = logger;
    }

    protected override Task HandleAsync(WeatherForecast @event, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Weather forecast received: {Date} - {TemperatureC}°C, {Summary}",
            @event.Date.ToShortDateString(),
            @event.TemperatureC,
            @event.Summary);

        return Task.CompletedTask;
    }
}
