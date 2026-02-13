namespace Functions.Functions;

using Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

/// <summary>
/// A function that triggers when there is a new file created and process the file before output to the storage.
/// Note: The container needs to be different to prevent circular trigger.
/// </summary>
public sealed class BlobTriggerOutputFunction
{
    private const string InputContainer = Constants.TestBlobContainerName;
    private const string OutputContainer = "processed";
    private const string InputContainerExistingFile = "must-exist.txt";
    private readonly ILogger<BlobTriggerOutputFunction> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobTriggerOutputFunction"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public BlobTriggerOutputFunction(ILogger<BlobTriggerOutputFunction> logger)
    {
        this.logger = Guard.ThrowIfNull(logger);
    }

    /// <summary>
    /// The function entry point.
    /// </summary>
    /// <param name="stream">The stream of the content of the new file.</param>
    /// <param name="unused">The unused stream of the existing file.</param>
    /// <param name="name">The name of the new file.</param>
    /// <returns>The processed content.</returns>
    [Function(nameof(BlobTriggerOutputFunction))]
    [BlobOutput(OutputContainer + "/{name}-processed.txt")]
    public async Task<string> Run(
        [BlobTrigger(InputContainer + "/{name}", Connection = "")] Stream stream,
        [BlobInput($"{InputContainer}/{InputContainerExistingFile}")] Stream unused, // This line is necessary for the function to work as intended. The file must be an existing file in the container.
        string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();
        this.logger.LogInformation("Blob trigger function processed blob\n Name: {Name} \n Data: {Content}", name, content);

        return "hello" + content;
    }
}
