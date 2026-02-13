namespace Blobs;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;
using Core;

/// <summary>
/// The blob storage service repository.
/// </summary>
internal sealed class BlobService
{
    private readonly BlobContainerClient container;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlobService"/> class.
    /// </summary>
    /// <param name="client">The blob service client object.</param>
    public BlobService(BlobServiceClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        this.container = client.GetBlobContainerClient(Constants.TestBlobContainerName);
        this.container.CreateIfNotExists();
    }

    /// <summary>
    /// Demonstrates whether the blob exist by it's name.
    /// </summary>
    /// <param name="name">The blob name.</param>
    /// <returns>The flag that indicates whether it exists.</returns>
    public async Task<bool> BlobExistsAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var blob = this.container.GetBlobClient(name);
        var result = await blob.ExistsAsync();

        return result.Value;
    }

    /// <summary>
    /// Demo to retrieve the binary content of the blob.
    /// </summary>
    /// <param name="name">The name of the blob.</param>
    /// <returns>The binary data object.</returns>
    public async Task<BinaryData> GetBlobBinaryDataAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var blob = this.container.GetBlobClient(name);
        var download = await blob.DownloadContentAsync();

        return download.Value.Content;
    }

    /// <summary>
    /// Demo to list all blobs inside the container.
    /// </summary>
    /// <returns>The list of blob names that exist.</returns>
    public async Task<IEnumerable<string>> ListBlobsAsync()
    {
        var items = new List<string>();

        await foreach (BlobItem blobItem in this.container.GetBlobsAsync())
        {
            items.Add(blobItem.Name);
        }

        return items;
    }

    /// <summary>
    /// Demo to upload file as a blob from its file path.
    /// </summary>
    /// <param name="filePath">The file path of the file on disk.</param>
    /// <param name="fileName">The name of the file after uploaded.</param>
    /// <returns>The asynchronous task.</returns>
    public Task UploadFileBlobAsync(string filePath, string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        // At this point, the blob is not exist yet but we can get a reference to it
        var blobClient = this.container.GetBlobClient(fileName);

        return blobClient.UploadAsync(filePath, new BlobHttpHeaders
        {
            ContentType = filePath.GetContentType(),
        });
    }

    /// <summary>
    /// Uploads a blob file using string content.
    /// </summary>
    /// <param name="content">The content of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The asynchronous task.</returns>
    public async Task UploadContentBlobAsync(string content, string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var blobClient = this.container.GetBlobClient(fileName);
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        await blobClient.UploadAsync(stream, new BlobHttpHeaders
        {
            ContentType = fileName.GetContentType(),
        });
    }

    /// <summary>
    /// Uploads a blob file using string content.
    /// </summary>
    /// <param name="content">The content of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The asynchronous task.</returns>
    public Task UploadContentBlobV2Async(string content, string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return this.container.UploadBlobAsync(fileName, new BinaryData(content));
    }

    /// <summary>
    /// Deletes a blob file by its name.
    /// </summary>
    /// <param name="blobName">The blob name.</param>
    /// <returns>The asynchronous task.</returns>
    public Task DeleteBlobAsync(string blobName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        var blobClient = this.container.GetBlobClient(blobName);
        return blobClient.DeleteIfExistsAsync();
    }
}
