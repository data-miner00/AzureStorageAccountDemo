using Azure.Data.Tables;
using Azure.Data.Tables.Models;

namespace Tables
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var tableName = "testtable";
            var serviceClient = new TableServiceClient(Constants.StorageAccountConnectionString);

            TableItem table = serviceClient.CreateTableIfNotExists(tableName);
            Console.WriteLine($"The created table's name is {table.Name}.");

            var tableClient = serviceClient.GetTableClient(tableName);

            string partitionKey = "user", rowKey = "id";

            var tableEntity = new TableEntity(partitionKey, rowKey)
            {
                { "name", "John" },
                { "age", 30 },
                { "email", "john@email.com" }
            };

            tableClient.AddEntity(tableEntity);
        }
    }
}
