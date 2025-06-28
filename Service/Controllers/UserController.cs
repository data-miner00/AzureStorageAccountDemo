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

    private readonly ILogger<UserController> logger;
    private readonly MessagePublisher publisher;
    private readonly UserGenerator userGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="publisher">The message publisher.</param>
    /// <param name="userGenerator">The user generator.</param>
    public UserController(
        ILogger<UserController> logger,
        MessagePublisher publisher,
        UserGenerator userGenerator)
    {
        this.logger = Guard.ThrowIfNull(logger);
        this.publisher = Guard.ThrowIfNull(publisher);
        this.userGenerator = Guard.ThrowIfNull(userGenerator);
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
            var user = await this.userGenerator.GenerateAsync();
            await this.publisher.PublishAsync(QueueName, user);
        }

        return this.Created();
    }
}
