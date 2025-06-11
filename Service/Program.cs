namespace Service;

using Azure.Storage.Queues;
using Service.Services;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddHostedService<LogBackgroundService>();
        builder.Services.AddHostedService<QueueListenerBackgroundService>();

        builder.AddAzureQueueClient();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    private static WebApplicationBuilder AddAzureQueueClient(this WebApplicationBuilder builder)
    {
        var connection = builder.Configuration["ConnectionStrings:Default"]
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        var serviceClient = new QueueServiceClient(connection);

        var queue = serviceClient.GetQueueClient("sample")
            ?? throw new InvalidOperationException("Queue does not found.");

        builder.Services.AddSingleton(queue);

        return builder;
    }
}
