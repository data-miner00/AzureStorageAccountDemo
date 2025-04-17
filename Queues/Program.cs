namespace Queues
{
    using Azure.Storage.Queues;
    using Azure.Messaging;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var queueName = "myqueue-items";
            var client = new QueueServiceClient(Constants.StorageAccountConnectionString);
            var application = new Application(client, queueName);

            var queue = await application.GetQueueAsync(true);

            await application.SendMessageAsync("Hello World");

            var user = new User
            {
                Name = "Shaun",
                Age = 27,
                Emails = ["shaun.chong@email.com", "shaun@mail.net"],
            };

            await application.SendMessageAsync(user);

            await application.PeekAsync();
        }
    }
}
