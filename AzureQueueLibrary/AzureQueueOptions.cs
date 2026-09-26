namespace AzureQueueLibrary;

public class AzureQueueOptions
{
    public const string Position = "AzureStorageQueue";
    public string DefaultQueueName { get; set; } = string.Empty;
}
