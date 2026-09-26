using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;

namespace AzureQueueLibrary;

public class AzureQueueService : IAzureQueueService
{
    private readonly QueueServiceClient _queueServiceClient;
    private readonly AzureQueueOptions _options;
    private readonly ConcurrentDictionary<string, QueueClient> _queueClients = new();

    public AzureQueueService(QueueServiceClient queueServiceClient, IOptions<AzureQueueOptions> options)
    {
        _queueServiceClient = queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    private QueueClient GetQueueClient(string? queueName)
    {
        var targetQueue = queueName ?? _options.DefaultQueueName;
        if (string.IsNullOrWhiteSpace(targetQueue))
        {
            throw new InvalidOperationException("Kuyruk adı belirtilmedi ve varsayılan kuyruk adı (DefaultQueueName) ayarlanmadı.");
        }

        return _queueClients.GetOrAdd(targetQueue.ToLowerInvariant(), qName =>
        {
            var client = _queueServiceClient.GetQueueClient(qName);
            client.CreateIfNotExists();
            return client;
        });
    }

    public async Task SendMessageAsync<T>(T message, string? queueName = null, TimeSpan? visibilityTimeout = null, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default)
    {
        var client = GetQueueClient(queueName);
        var jsonMessage = JsonSerializer.Serialize(message);
        await client.SendMessageAsync(jsonMessage, visibilityTimeout, timeToLive, cancellationToken);
    }

    public async Task<T?> ReceiveMessageAsync<T>(string? queueName = null, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default)
    {
        var client = GetQueueClient(queueName);
        QueueMessage[] messages = await client.ReceiveMessagesAsync(maxMessages: 1, visibilityTimeout: visibilityTimeout, cancellationToken: cancellationToken);

        var message = messages.FirstOrDefault();
        if (message is null) return default;

        return JsonSerializer.Deserialize<T>(message.MessageText);
    }

    public async Task<IReadOnlyCollection<T>> ReceiveMessagesAsync<T>(int maxMessages = 10, string? queueName = null, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default)
    {
        var client = GetQueueClient(queueName);
        QueueMessage[] messages = await client.ReceiveMessagesAsync(maxMessages: maxMessages, visibilityTimeout: visibilityTimeout, cancellationToken: cancellationToken);

        var list = new List<T>();
        foreach (var msg in messages)
        {
            var deserialized = JsonSerializer.Deserialize<T>(msg.MessageText);
            if (deserialized is not null)
            {
                list.Add(deserialized);
            }
        }

        return list.AsReadOnly();
    }

    public async Task DeleteMessageAsync(string messageId, string popReceipt, string? queueName = null, CancellationToken cancellationToken = default)
    {
        var client = GetQueueClient(queueName);
        await client.DeleteMessageAsync(messageId, popReceipt, cancellationToken);
    }

    public async Task ClearQueueAsync(string? queueName = null, CancellationToken cancellationToken = default)
    {
        var client = GetQueueClient(queueName);
        await client.ClearMessagesAsync(cancellationToken);
    }
}
