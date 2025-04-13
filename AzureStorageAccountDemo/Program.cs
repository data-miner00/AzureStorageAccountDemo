using Azure.Storage.Blobs;

namespace AzureStorageAccountDemo
{
    internal class Program
    {
        internal const string StorageAccountName = "devstoreaccount1";
        internal const string StorageAccountConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10003/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        static async Task Main(string[] args)
        {
            var client = new BlobServiceClient(StorageAccountConnectionString);

            var response = await client.CreateBlobContainerAsync("testcontainer");

            var blobClient = response.Value;

            await blobClient.UploadBlobAsync("test.txt", new BinaryData("Hello, World!"));
        }
    }
}
