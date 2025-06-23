namespace Service.Controllers;

using Core;

using Microsoft.AspNetCore.Mvc;
using Service.Publishers;
using Service.Results;

/// <summary>
/// The controller for managing weather forecasts.
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class WeatherForecastController : ControllerBase
{
    private const string QueueName = "weather";

    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> logger;
    private readonly MessagePublisher publisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="publisher">The publisher.</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger, MessagePublisher publisher)
    {
        this.logger = Guard.ThrowIfNull(logger);
        this.publisher = Guard.ThrowIfNull(publisher);
    }

    /// <summary>
    /// Gets the weather forecast data.
    /// </summary>
    /// <returns>The list of forecast data.</returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        this.logger.LogInformation("Fetching weather forecast data.");

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        })
        .ToArray();
    }

    /// <summary>
    /// Simulates an error for testing purposes.
    /// </summary>
    /// <returns>The action result.</returns>
    /// <exception cref="InvalidOperationException">An arbitrary exception.</exception>
    [HttpGet("error")]
    public IActionResult GetError()
    {
        throw new InvalidOperationException("This is a test error.");
    }

    /// <summary>
    /// Publishes a weather forecast message to the queue.
    /// </summary>
    /// <param name="weather">The weather forecast data.</param>
    /// <returns>The action result.</returns>
    [HttpPost("publish")]
    public async Task<IActionResult> PublishMessage([FromBody] WeatherForecast weather)
    {
        this.logger.LogInformation("Publishing weather forecast message.");

        await this.publisher.PublishAsync(QueueName, weather);

        return this.Created();
    }

    /// <summary>
    /// Publishes multiple weather forecast messages to the queue.
    /// </summary>
    /// <param name="count">The count of the message.</param>
    /// <returns>The action result.</returns>
    [HttpPost("publish/{count:maxCount}")]
    public async Task<IActionResult> PublishMultipleMessages(int count)
    {
        this.logger.LogInformation("Publishing {Count} weather forecast messages.", count);
        for (int i = 0; i < count; i++)
        {
            var weather = new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            };
            await this.publisher.PublishAsync(QueueName, weather);
        }

        return this.Created();
    }

    [HttpGet("html")]
    public async Task<IResult> GetHtml()
    {
        this.logger.LogInformation("Fetching HTML content.");
        var htmlContent = "<html><body><h1>Weather Forecast</h1><p>This is a sample HTML response.</p></body></html>";
        return new HtmlResult(htmlContent);
    }
}
