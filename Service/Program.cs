namespace Service;

using Azure.Storage.Queues;
using Scalar.AspNetCore;
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
        builder.Services.AddProblemDetails(opt =>
        {
            opt.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Extensions.Add("additionalInfo", "hello world");
                context.ProblemDetails.Extensions.Add("server", Environment.MachineName);
            };
        });

        builder.AddAzureQueueClient();

        var app = builder.Build();

        if (app.Environment.IsProduction())
        {
            app.UseExceptionHandler();
            app.UseHsts();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/openapi/v1.json", "Queue Listener API V1"); // /swagger/index
            });
            app.MapScalarApiReference(opt => // /scalar/v1
            {
                opt
                    .WithTitle("Queue Listener API V1")
                    .WithTheme(ScalarTheme.Solarized)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
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
