namespace Service.TableEntities;

using Azure;
using Azure.Data.Tables;
using System;

/// <summary>
/// The weather forecast entity for Azure Table Storage.
/// </summary>
public class WeatherForecastEntity : ITableEntity
{
    /// <inheritdoc/>
    public string PartitionKey { get; set; }

    /// <inheritdoc/>
    public string RowKey { get; set; }

    /// <inheritdoc/>
    public DateTimeOffset? Timestamp { get; set; }

    /// <inheritdoc/>
    public ETag ETag { get; set; }

    /// <summary>
    /// Gets or sets the date of the forecast.
    /// </summary>
    public DateTime Date { get; set; }

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
