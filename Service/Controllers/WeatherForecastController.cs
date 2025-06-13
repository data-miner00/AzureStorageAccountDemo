namespace Service.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        this.logger = logger;
    }

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

    [HttpGet("error")]
    public IActionResult GetError()
    {
        throw new InvalidOperationException("This is a test error.");
    }
}
