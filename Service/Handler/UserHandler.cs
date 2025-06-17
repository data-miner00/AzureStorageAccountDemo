namespace Service.Handler;

using Core;
using Service.Attributes;
using System.Threading;
using System.Threading.Tasks;

[Handler("user")]
public class UserHandler : MessageHandler<User>
{
    private readonly ILogger<UserHandler> logger;

    public UserHandler(ILogger<UserHandler> logger)
    {
        this.logger = logger;
    }

    public override Task HandleAsync(object @event, CancellationToken cancellationToken)
    {
        //this.logger.LogInformation("User event received: {FirstName} - {LastName} - {Emails}",
        //    @event.FirstName,
        //    @event.LastName,
        //    @event.Emails);

        Console.WriteLine();

        return Task.CompletedTask;
    }
}
