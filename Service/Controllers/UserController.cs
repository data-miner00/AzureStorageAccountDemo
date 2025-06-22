namespace Service.Controllers;

using Core;
using Microsoft.AspNetCore.Mvc;
using Service.Publishers;

/// <summary>
/// The controller for managing users.
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private const string QueueName = "user";

    private static readonly string[] UserNames =
    [
        "Alice", "Bob", "Charlie", "David", "Eve"
    ];

    private readonly ILogger<UserController> logger;
    private readonly MessagePublisher publisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="publisher">The message publisher.</param>
    public UserController(ILogger<UserController> logger, MessagePublisher publisher)
    {
        this.logger = Guard.ThrowIfNull(logger);
        this.publisher = Guard.ThrowIfNull(publisher);
    }

    /// <summary>
    /// Publishes a user message to the queue.
    /// </summary>
    /// <param name="user">The user to be published.</param>
    /// <returns>The action result.</returns>
    [HttpPost("publish")]
    public async Task<IActionResult> PublishMessage([FromBody] User user)
    {
        this.logger.LogInformation("Publishing user message.");
        await this.publisher.PublishAsync(QueueName, user);
        return this.Created();
    }

    /// <summary>
    /// Publishes multiple user messages to the queue.
    /// </summary>
    /// <param name="count">The number of messages.</param>
    /// <returns>The action result.</returns>
    [HttpPost("publish/{count:maxCount}")]
    public async Task<IActionResult> PublishMultipleMessages(int count)
    {
        this.logger.LogInformation("Publishing multiple user messages.");
        for (int i = 0; i < count; i++)
        {
            var user = new User
            {
                FirstName = UserNames[Random.Shared.Next(UserNames.Length)],
                LastName = $"User{i + 1}",
                Emails = [$"{UserNames[Random.Shared.Next(UserNames.Length)].ToLower()}@example.com"],
            };
            await this.publisher.PublishAsync(QueueName, user);
        }

        return this.Created();
    }
}
