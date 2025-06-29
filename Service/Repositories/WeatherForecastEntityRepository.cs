namespace Service.Repositories;

using Azure;
using Azure.Data.Tables;
using Core;
using Service.TableEntities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Weather forecast entity repository for Azure Table.
/// </summary>
public class WeatherForecastEntityRepository : IRepository<WeatherForecastEntity>
{
    private const string PartitionKey = "WeatherForecast";

    private readonly ILogger<WeatherForecastEntityRepository> logger;
    private readonly TableClient tableClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastEntityRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="tableClient">The weather forecast table client.</param>
    public WeatherForecastEntityRepository(ILogger<WeatherForecastEntityRepository> logger, TableClient tableClient)
    {
        this.logger = Guard.ThrowIfNull(logger);
        this.tableClient = Guard.ThrowIfNull(tableClient);
    }

    /// <inheritdoc/>
    public async Task CreateAsync(WeatherForecastEntity entity, CancellationToken cancellationToken)
    {
        Guard.ThrowIfNull(entity);

        await this.tableClient.AddEntityAsync(entity, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
    {
        Guard.ThrowIfNullOrWhitespace(id);

        WeatherForecastEntity entity = await this.tableClient.GetEntityAsync<WeatherForecastEntity>(PartitionKey, id, cancellationToken: cancellationToken);

        await this.tableClient.DeleteEntityAsync(entity, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<WeatherForecastEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<WeatherForecastEntity> entities = [];

        AsyncPageable<WeatherForecastEntity> queryResultsFilter = this.tableClient.QueryAsync<WeatherForecastEntity>(filter: $"PartitionKey eq '{PartitionKey}'");

        await foreach (WeatherForecastEntity entity in queryResultsFilter)
        {
            entities.Add(entity);
        }

        return entities;
    }

    /// <inheritdoc/>
    public async Task<WeatherForecastEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        Guard.ThrowIfNullOrWhitespace(id);

        var entity = await this.tableClient.GetEntityAsync<WeatherForecastEntity>(PartitionKey, id, cancellationToken: cancellationToken);

        return entity.Value;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(WeatherForecastEntity entity, CancellationToken cancellationToken)
    {
        Guard.ThrowIfNull(entity);

        await this.tableClient.UpdateEntityAsync(entity, entity.ETag, cancellationToken: cancellationToken);
    }
}
