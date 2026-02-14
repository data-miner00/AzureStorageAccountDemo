namespace Core;

/// <summary>
/// The global shared constants.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The storage account name.
    /// </summary>
    public const string StorageAccountName = "devstoreaccount1";

    /// <summary>
    /// The storage account connection string.
    /// </summary>
    public const string StorageAccountConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10003/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    /// <summary>
    /// The test blob container name.
    /// </summary>
    public const string TestBlobContainerName = "testcontainer";

    /// <summary>
    /// The test queue name.
    /// </summary>
    public const string TestQueueName = "testqueue";

    /// <summary>
    /// The test Azure Table name.
    /// </summary>
    public const string TestTableName = "testtable";
}
