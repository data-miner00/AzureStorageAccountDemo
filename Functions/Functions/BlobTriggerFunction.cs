namespace Functions.Functions;

using Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

/// <summary>
/// A function that triggers when there is a new file created in the blob container.
/// </summary>
public sealed class BlobTriggerFunction
{
    private readonly ILogger<BlobTriggerFunction> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobTriggerFunction"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public BlobTriggerFunction(ILogger<BlobTriggerFunction> logger)
    {
        this.logger = Guard.ThrowIfNull(logger);
    }

    /// <summary>
    /// The function entry point.
    /// </summary>
    /// <param name="stream">The stream of the content of the new file.</param>
    /// <param name="name">The name of the new file.</param>
    /// <returns>The asynchronous task.</returns>
    [Function(nameof(BlobTriggerFunction))]
    public async Task Run([BlobTrigger(Constants.TestBlobContainerName + "/{name}", Connection = "")] Stream stream, string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();
        this.logger.LogInformation("Blob trigger function processed blob\n Name: {Name} \n Data: {Content}", name, content);
    }
}
