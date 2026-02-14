namespace Tables;

using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Core;

/// <summary>
/// The program structure.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The entry point.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args)
    {
        var serviceClient = new TableServiceClient(Constants.StorageAccountConnectionString);

        TableItem table = serviceClient.CreateTableIfNotExists(Constants.TestTableName);
        Console.WriteLine($"The created table's name is {table.Name}.");

        var tableClient = serviceClient.GetTableClient(Constants.TestTableName);

        string partitionKey = "user", rowKey = Guid.NewGuid().ToString();

        var tableEntity = new TableEntity(partitionKey, rowKey)
        {
            { "name", "John" },
            { "age", new Random().Next(100) },
            { "email", "john@email.com" },
        };

        tableClient.AddEntity(tableEntity);

        Console.WriteLine($"{tableEntity.RowKey}: {tableEntity["name"]} is {tableEntity.GetInt32("age")} years old.");

        Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'");

        foreach (TableEntity entity in queryResultsFilter)
        {
            Console.WriteLine($"{entity.GetString("name")}: {entity.GetInt32("age")}");
        }

        Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");

        // Using strongly typed entity
        var user = new User
        {
            Name = "John",
            Age = new Random().Next(100),
            Emails = string.Join(',', "john@email.com", "johnemail2@email.com"),
            PartitionKey = partitionKey,
            RowKey = Guid.NewGuid().ToString(),
        };

        tableClient.AddEntity(user);

        int middleAge = 50;
        Pageable<User> queryResultsLINQ = tableClient.Query<User>(user => user.Age >= middleAge);

        foreach (var entity in queryResultsLINQ)
        {
            Console.WriteLine($"{entity.Name}: {entity.Age}");
        }

        Console.WriteLine($"The query returned {queryResultsLINQ.Count()} entities.");

        // Delete the entity
        tableClient.DeleteEntity(partitionKey, rowKey);
        Console.WriteLine($"Deleted {partitionKey} - {rowKey}");
    }
}
