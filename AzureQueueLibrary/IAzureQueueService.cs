namespace AzureQueueLibrary;

public interface IAzureQueueService
{
    Task SendMessageAsync<T>(T message, string? queueName = null, TimeSpan? visibilityTimeout = null, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default);
    Task<T?> ReceiveMessageAsync<T>(string? queueName = null, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> ReceiveMessagesAsync<T>(int maxMessages = 10, string? queueName = null, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default);
    Task DeleteMessageAsync(string messageId, string popReceipt, string? queueName = null, CancellationToken cancellationToken = default);
    Task ClearQueueAsync(string? queueName = null, CancellationToken cancellationToken = default);
}
