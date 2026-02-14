namespace Queues;

using Azure.Storage.Queues;
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
    /// <returns>The asynchronous task.</returns>
    public static async Task Main(string[] args)
    {
        var client = new QueueServiceClient(Constants.StorageAccountConnectionString);
        var application = new Application(client, Constants.TestQueueName);
        var userGenerator = new UserGenerator();

        var queue = await application.GetQueueAsync(true);

        await application.SendMessageAsync("Hello World");

        var user = await userGenerator.GenerateAsync();

        await application.SendMessageAsync(user);

        await application.PeekAsync();
    }
}
