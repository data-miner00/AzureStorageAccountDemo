namespace Service.Handler;

using Core;
using Service.Attributes;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The message handler for <see cref="User"/> event.
/// </summary>
[Handler("user")]
public class UserHandler : MessageHandler<User>
{
    private readonly ILogger<UserHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UserHandler(ILogger<UserHandler> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    protected override Task HandleAsync(User @event, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "User event received: {FirstName} - {LastName} - {Emails}",
            @event.FirstName,
            @event.LastName,
            @event.Emails);

        return Task.CompletedTask;
    }
}
