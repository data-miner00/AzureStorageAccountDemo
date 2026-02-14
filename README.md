# AzureStorageAccountDemo

Demo for Azure Storage Account using Azurite.

## Blob Storage

1. Use to serve web assets
1. Distributed access
1. Video streaming storage
1. Backups
1. Storage to analyze later
1. Uses virtual directory (separated by `/` in the name)

### Copying across containers and folders

To copy the contents of a container to another container or folder, we can use [azcopy](https://github.com/Azure/azure-storage-azcopy) command. 

```bash
azcopy copy <source-path> <destination-path> --recursive
```

Copy from local to a container: Generate the SAS token for the container and use it in the destination path.

```bash
azcopy copy "C:\local\path" "https://account.blob.core.windows.net/mycontainer1/?sv=2018-03-28&ss=bjqt&srt=sco&sp=rwddgcup&se=2019-05-01T05:01:17Z&st=2019-04-30T21:01:17Z&spr=https&sig=MGCXiyEzbtttkr3ewJIh2AR8KrghSy1DGM9ovN734bQF4%3D" --recursive=true
```

Copy from container to another container:

```bash
azcopy copy "https://account.blob.core.windows.net/mycontainer1/?sv=2018-03-28&ss=bjqt&srt=sco&sp=rwddgcup&se=2019-05-01T05:01:17Z&st=2019-04-30T21:01:17Z&spr=https&sig=MGCXiyEzbtttkr3ewJIh2AR8KrghSy1DGM9ovN734bQF4%3D" "https://account.blob.core.windows.net/mycontainer2/?sv=2018-03-28&ss=bjqt&srt=sco&sp=rwddgcup&se=2019-05-01T05:01:17Z&st=2019-04-30T21:01:17Z&spr=https&sig=MGCXiyEzbtttkr3ewJIh2AR8KrghSy1DGM9ovN734bQF4%3D" --recursive=true
```

Copy from container to a virtual folder within another container: Use `/*` to specify in the source and `/folderName` to specify the folder in the destination.

```bash
azcopy copy "https://account.blob.core.windows.net/mycontainer1/*?sv=2018-03-28&ss=bjqt&srt=sco&sp=rwddgcup&se=2019-05-01T05:01:17Z&st=2019-04-30T21:01:17Z&spr=https&sig=MGCXiyEzbtttkr3ewJIh2AR8KrghSy1DGM9ovN734bQF4%3D" "https://account.blob.core.windows.net/mycontainer2/myfolder1?sv=2018-03-28&ss=bjqt&srt=sco&sp=rwddgcup&se=2019-05-01T05:01:17Z&st=2019-04-30T21:01:17Z&spr=https&sig=MGCXiyEzbtttkr3ewJIh2AR8KrghSy1DGM9ovN734bQF4%3D" --recursive=true
```

### Security Concerns

## Queue Storage

1. Store large number of messages
1. Sub-service of Azure Storage Account
1. Up to 500 TiB of data
1. Max 2,000 (1 KiB) messages/s
1. Decouple applications
1. Achieve Queue-based Load Levelling pattern
1. Achieve Competing Consumers pattern (fan-out)
1. Achieve Retry pattern

Message
- Data in any format
- Up to 64 KiB
- Default TTL is 7 days
- Non-expiring messages TTL -1

Supported Authorization Methods
- Account key
- Shared Access Signature (SAS)
- Azure Active Directory with RBAC roles
  - Storage Queue Data Contributor
  - Storage Queue Data Reader
  - Storage Queue Data Message Processor
  - Storage Queue Data Message Sender

Important
- Single queue options are atomic
- Don't support transactions
- No batching

## Table Storage

1. Does not support array, list. Serialize them into comma delimited string instead.

## Prerequisites

- Docker
- Node.js
- npm
- Azurite
- [Azure Storage Explorer](https://github.com/microsoft/AzureStorageExplorer)
- Azure CLI

## Todo

- Write in an article to explain the steps needed to work with blob storage, queues, and tables using Azurite.
- Modularize this into interactive app

## Notes

- If a blob function is newly created, it will trigger the function to run for the preexisting blobs in the container.
- For my case, the port 10000 was occupied and I need to change the blob port to 10003.

## Links

- [Use the Azurite emulator for local Azure Storage development](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage)
- [Tutorial: Trigger Azure Functions on blob containers by using an event subscription](https://learn.microsoft.com/en-us/azure/azure-functions/functions-event-grid-blob-trigger?pivots=programming-language-csharp)
- [Azurite](https://github.com/Azure/Azurite)
- [Getting started with Azure Blob Storage in .NET Core | Azure Tutorial](https://www.youtube.com/watch?v=9ZpMpf9dNDA)
- [A Beginners Guide to Azure Blob Storage](https://www.youtube.com/watch?v=ah1XqItWkuc)
- [Azure Queue Storage Tutorial](https://www.youtube.com/watch?v=JQ6KhjU5Zsg)
- [Create an Azure table](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/data.tables-readme?view=azure-dotnet#create-an-azure-table)
- [Tables, Entities, and Properties](https://learn.microsoft.com/en-us/rest/api/storageservices/understanding-the-table-service-data-model#tables-entities-and-properties)
- [Azure Data Tables samples](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/tables/Azure.Data.Tables/samples)
- [Azure Blob storage trigger for Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob-trigger?tabs=python-v2%2Cin-process%2Cnodejs-v4%2Cextensionv5&pivots=programming-language-csharp)
