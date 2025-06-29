namespace Service.Controllers;

using Core;

using Microsoft.AspNetCore.Mvc;
using Service.Publishers;
using Service.Repositories;
using Service.Results;
using Service.TableEntities;

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
    private readonly IRepository<WeatherForecastEntity> repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="publisher">The publisher.</param>
    /// <param name="repository">The table repository.</param>
    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        MessagePublisher publisher,
        IRepository<WeatherForecastEntity> repository)
    {
        this.logger = Guard.ThrowIfNull(logger);
        this.publisher = Guard.ThrowIfNull(publisher);
        this.repository = Guard.ThrowIfNull(repository);
    }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken => this.HttpContext.RequestAborted;

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

    /// <summary>
    /// Get HTML content for testing purposes.
    /// </summary>
    /// <returns>The Html Result.</returns>
    [HttpGet("html")]
    public Task<IResult> GetHtml()
    {
        this.logger.LogInformation("Fetching HTML content.");
        var htmlContent = "<html><body><h1>Weather Forecast</h1><p>This is a sample HTML response.</p></body></html>";
        return Task.FromResult<IResult>(new HtmlResult(htmlContent));
    }

    /// <summary>
    /// Creates a new weather forecast record in the repository.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    /// <returns>The action result.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] WeatherForecastEntity entity)
    {
        this.logger.LogInformation("Creating a new weather forecast entity.");
        await this.repository.CreateAsync(entity, this.CancellationToken);
        return this.CreatedAtAction(nameof(this.Get), new { id = entity.RowKey }, entity);
    }

    /// <summary>
    /// Gets a weather forecast entity by its ID.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <returns>The action result.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        this.logger.LogInformation("Fetching weather forecast entity by ID: {Id}", id);
        var entity = await this.repository.GetByIdAsync(id, this.CancellationToken);
        if (entity == null)
        {
            return this.NotFound();
        }

        return this.Ok(entity);
    }

    /// <summary>
    /// Updates an existing weather forecast entity by its ID.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="entity">The entity to be updated.</param>
    /// <returns>The action result.</returns>
    /// <exception cref="InvalidOperationException">When rowkey is not equal to id passed in the route.</exception>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] WeatherForecastEntity entity)
    {
        this.logger.LogInformation("Updating weather forecast entity by ID: {Id}", id);
        if (entity.RowKey != id)
        {
            throw new InvalidOperationException("The RowKey is not equal to the id.");
        }

        await this.repository.UpdateAsync(entity, this.CancellationToken);

        return this.NoContent();
    }

    /// <summary>
    /// Deletes a weather forecast entity by its ID.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <returns>The action result.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        this.logger.LogInformation("Deleting weather forecast entity by ID: {Id}", id);
        await this.repository.DeleteByIdAsync(id, this.CancellationToken);
        return this.NoContent();
    }
}
