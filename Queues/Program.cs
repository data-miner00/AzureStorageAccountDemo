namespace Queues;

using Azure.Storage.Queues;
using Core;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var client = new QueueServiceClient(Constants.StorageAccountConnectionString);
        var application = new Application(client, Constants.TestQueueName);
        var userGenerator = new UserGenerator();

        var queue = await application.GetQueueAsync(true);

        await application.SendMessageAsync("Hello World");

        var user = userGenerator.GenerateAsync().GetAwaiter().GetResult();

        await application.SendMessageAsync(user);

        await application.PeekAsync();
    }
}
