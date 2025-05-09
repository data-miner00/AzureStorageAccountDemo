namespace Tables;

using Azure;
using Azure.Data.Tables;
using System.Runtime.Serialization;

internal class User : ITableEntity
{
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "age")]
    public int Age { get; set; }

    [DataMember(Name = "emails")]
    public string Emails { get; set; }

    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }
}
