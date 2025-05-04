namespace Queues
{
    using Azure.Storage.Queues;
    using Azure.Messaging;
    using Core;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var queueName = "myqueue-items";
            var client = new QueueServiceClient(Constants.StorageAccountConnectionString);
            var application = new Application(client, queueName);
            var userGenerator = new UserGenerator();

            var queue = await application.GetQueueAsync(true);

            await application.SendMessageAsync("Hello World");

            var user = userGenerator.GenerateAsync().GetAwaiter().GetResult();

            await application.SendMessageAsync(user);

            await application.PeekAsync();
        }
    }
}
