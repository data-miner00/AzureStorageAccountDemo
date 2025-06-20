namespace Core;

/// <summary>
/// The weather forecast result.
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// Gets or sets the date of the forecast.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the temperature in degree celcius.
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Gets the temperature in degree farenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

    /// <summary>
    /// Gets or sets the summary of the forecast.
    /// </summary>
    public string? Summary { get; set; }
}
