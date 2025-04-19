using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blobs
{
    internal class BlobService
    {
        private readonly BlobServiceClient client;
        private readonly BlobContainerClient container;

        public BlobService(BlobServiceClient client)
        {
            this.client = client;
            this.container = client.GetBlobContainerClient(Constants.BlobContainer);
        }

        public Task<BlobInfo> GetBlobAsync(string name)
        {
            var blob = container.GetBlobClient(name);
            return blob.ExistsAsync().ContinueWith(t =>
            {
                if (t.Result.Value)
                {
                    return Activator.CreateInstance<BlobInfo>();
                }
                else
                {
                    throw new Exception($"Blob {name} does not exist.");
                }
            });
        }

        public async Task<BinaryData> GetBlobAsync2(string name)
        {
            var blob = container.GetBlobClient(name);
            var download = await blob.DownloadContentAsync();

            return download.Value.Content;
        }

        public async Task<IEnumerable<string>> ListBlobsAsync()
        {
            var items = new List<string>();

            await foreach (BlobItem blobItem in container.GetBlobsAsync())
            {
                items.Add(blobItem.Name);
            }

            return items;
        }

        public async Task UploadFileBlobAsync(string filePath, string fileName)
        {
            var blobClient = container.GetBlobClient(fileName); // At this point, the blob is not exist yet but we can get a reference to it
            await blobClient.UploadAsync(filePath, new BlobHttpHeaders { ContentType = filePath.GetContentType() });

        }

        public async Task UploadContentBlobAsync(string content, string fileName)
        {
            var blobClient = container.GetBlobClient(fileName);
            await using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = fileName.GetContentType() });
            }
        }

        public Task DeleteBlobAsync(string blobName)
        {
            var blobClient = container.GetBlobClient(blobName);
            return blobClient.DeleteIfExistsAsync();
        }
    }
}
