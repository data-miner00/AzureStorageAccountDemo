namespace Service;

using Azure.Storage.Queues;
using Scalar.AspNetCore;
using Service.Attributes;
using Service.Options;
using Service.Publishers;
using Service.Services;
using System.Reflection;

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

        builder.Services.AddSingleton<MessagePublisher>();
        builder.AddAzureQueueClient();
        builder.AddMessageHandlers();

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

        var option = builder.Configuration
            .GetSection(QueueOption.SectionName)
            .Get<QueueOption>()
            ?? throw new InvalidOperationException("Queue options are not configured.");

        builder.Services.AddSingleton(option);

        var serviceClient = new QueueServiceClient(connection);

        var typesWithAttribute = GetTypesWithHandlerAttribute();
        var queues = new Dictionary<string, QueueClient>();

        foreach (var type in typesWithAttribute)
        {
            var handlerAttribute = type.GetCustomAttribute<HandlerAttribute>()
                ?? throw new InvalidOperationException("Queue name not found.");

            var queueName = handlerAttribute.QueueName;

            if (string.IsNullOrEmpty(queueName))
            {
                throw new InvalidOperationException($"Handler {type.FullName} does not have a valid queue name.");
            }

            queues[queueName] = serviceClient.GetQueueClient(queueName);
        }

        builder.Services.AddSingleton(queues);

        return builder;
    }

    private static void AddMessageHandlers(this WebApplicationBuilder builder)
    {
        var typesWithAttribute = GetTypesWithHandlerAttribute();

        foreach (var type in typesWithAttribute)
        {
            Console.WriteLine(type.FullName);

            builder.Services.AddSingleton(type);
        }
    }

    private static IEnumerable<Type> GetTypesWithHandlerAttribute()
    {
        var attributeType = typeof(HandlerAttribute);
        var assembly = Assembly.GetExecutingAssembly();

        var typesWithAttribute = assembly.GetTypes()
            .Where(t => t.IsClass && t.GetCustomAttributes(attributeType, inherit: false).Length != 0);

        return typesWithAttribute;
    }
}
