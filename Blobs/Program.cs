namespace Blobs;

using Azure.Storage.Blobs;

/// <summary>
/// The program definition.
/// </summary>
internal static class Program
{
    private const string ConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10003/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
    private const string ContentToUpload = "Hello, World";

    private static readonly BlobService? BlobService;

    /// <summary>
    /// Initializes static members of the <see cref="Program"/> class.
    /// </summary>
    static Program()
    {
        if (BlobService == null)
        {
            var container = new BlobServiceClient(ConnectionString);
            BlobService = new BlobService(container);
        }
    }

    /// <summary>
    /// The entry point.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The asynchronous task.</returns>
    public static async Task Main(string[] args)
    {
        if (BlobService is null)
        {
            return;
        }

        await Console.Out.WriteLineAsync("Program started.");
        await BlobService.UploadContentBlobAsync(ContentToUpload, "content.txt");
        await BlobService.UploadContentBlobV2Async(ContentToUpload, "content-v2.txt");
        await Console.Out.WriteLineAsync("Uploaded two files.");

        var isExists = await BlobService.BlobExistsAsync("content.txt");
        await Console.Out.WriteLineAsync($"content.txt exists: {isExists}");

        var content = await BlobService.GetBlobBinaryDataAsync("content.txt");
        if (content is not null)
        {
            await Console.Out.WriteLineAsync(content.ToString());
        }

        var allFiles = await BlobService.ListBlobsAsync();
        foreach (var fileName in allFiles)
        {
            await Console.Out.WriteLineAsync(fileName);
            await BlobService.DeleteBlobAsync(fileName);
            await Console.Out.WriteLineAsync($"Deleted {fileName}");
        }
    }
}
