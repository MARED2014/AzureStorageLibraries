using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;

namespace AzureQueueLibrary;

public static class QueueCollectionExtensions
{
    /// <summary>
    /// <see cref="IAzureQueueService"/> bileşenini DI konteynerine ekler.
    /// <para>
    /// Bu metodun çalışabilmesi için <see cref="QueueServiceClient"/> nesnesinin DI konteynerinde önceden kaydolmuş olması gerekir.
    /// </para>
    /// </summary>
    /// <param name="services">Servislerin ekleneceği <see cref="IServiceCollection"/> nesnesi.</param>
    /// <param name="configureOptions"><see cref="AzureQueueOptions"/> ayarlarının yapılacağı konfigürasyon eylemi.</param>
    /// <returns>Chaining için yapılandırılmış <see cref="IServiceCollection"/> örneği.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> veya <paramref name="configureOptions"/> null olduğunda fırlatılır.</exception>
    /// <example>
    /// <code>
    /// // 1. QueueServiceClient kaydı (appsettings.json'dan connectionString ile):
    /// builder.Services.AddSingleton(new QueueServiceClient(builder.Configuration.GetConnectionString("AzureStorage"), new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }));
    ///
    /// // 2. Kütüphanenin kaydı:
    /// builder.Services.AddAzureQueueService(options =>
    /// {
    ///     options.DefaultQueueName = "order-processing-queue";
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddAzureQueueService(this IServiceCollection services, Action<AzureQueueOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        services.AddSingleton<IAzureQueueService, AzureQueueService>();

        return services;
    }

    /// <summary>
    /// Doğrudan connection string vererek <see cref="QueueServiceClient"/> ve <see cref="IAzureQueueService"/> servislerini birlikte kaydeder.
    /// </summary>
    /// <param name="services">Servislerin ekleneceği <see cref="IServiceCollection"/> nesnesi.</param>
    /// <param name="connectionString">Azure Storage hesabı bağlantı dizesi.</param>
    /// <param name="configureOptions"><see cref="AzureQueueOptions"/> ayarlarının yapılacağı konfigürasyon eylemi.</param>
    /// <returns>Chaining için yapılandırılmış <see cref="IServiceCollection"/> örneği.</returns>
    public static IServiceCollection AddAzureQueueService(this IServiceCollection services, string connectionString, Action<AzureQueueOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        services.AddSingleton(new QueueServiceClient(connectionString, new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64
        }));

        return services.AddAzureQueueService(configureOptions);
    }
}
