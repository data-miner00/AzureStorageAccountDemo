namespace Service.Handler;

using Service.Attributes;
using System.Threading;
using System.Threading.Tasks;

[Handler("weather")]
public class WeatherHandler : MessageHandler<WeatherForecast>
{
    private readonly ILogger<WeatherHandler> logger;

    public WeatherHandler(ILogger<WeatherHandler> logger)
    {
        this.logger = logger;
    }

    public override Task HandleAsync(object @event, CancellationToken cancellationToken)
    {
        //this.logger.LogInformation("Weather forecast received: {Date} - {TemperatureC}°C, {Summary}",
        //    @event.Date.ToShortDateString(),
        //    @event.TemperatureC,
        //    @event.Summary);

        Console.WriteLine();

        return Task.CompletedTask;
    }
}
