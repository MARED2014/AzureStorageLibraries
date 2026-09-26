using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;

namespace AzureTableLibrary;

public static class TableCollectionExtensions
{
    /// <summary>
    /// Azure Table Storage servislerini ve generic (ITableStorageService{TEntity}) repository yapısını DI konteynerine kaydeder.
    /// <para>
    /// Bu metot, arka planda (TableServiceClient) nesnesini singleton olarak kaydeder ve 
    /// tüm (ITableStorageService{TEntity}) bağımlılıklarını open-generic transient/scoped olarak çözer.
    /// </para>
    /// </summary>
    /// <param name="services">Servislerin ekleneceği (IServiceCollection) nesnesi.</param>
    /// <param name="connectionString">Azure Storage hesabı bağlantı dizesi (Connection String).</param>
    /// <returns>Chaining için yapılandırılmış (IServiceCollection) örneği.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services) veya <paramref name="connectionString"/> null olduğunda fırlatılır.</exception>
    /// <example>
    /// <code>
    /// // 1. Program.cs (DI Kaydı):
    /// builder.Services.AddAzureTableStorage(builder.Configuration.GetConnectionString("AzureStorage"));
    ///
    /// // 2. Entity Tanımı:
    /// public class CustomerEntity : ITableEntity
    /// {
    ///     public string PartitionKey { get; set; } = "Customers";
    ///     public string RowKey { get; set; } = Guid.NewGuid().ToString();
    ///     public string Name { get; set; }
    ///     public DateTimeOffset? Timestamp { get; set; }
    ///     public ETag ETag { get; set; }
    /// }
    ///
    /// // 3. Servis/Sınıf İçinde Kullanımı:
    /// public class CustomerService
    /// {
    ///     private readonly ITableStorageService<CustomerEntity> _tableService;
    ///
    ///     public CustomerService(ITableStorageService<CustomerEntity> tableService)
    ///     {
    ///         _tableService = tableService;
    ///     }
    ///
    ///     public async Task CreateCustomerAsync(string name)
    ///     {
    ///         var customer = new CustomerEntity { Name = name };
    ///         await _tableService.Add(customer);
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddAzureTableStorage(this IServiceCollection services, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        services.AddSingleton(new TableServiceClient(connectionString));
        services.AddScoped(typeof(ITableStorageService<>), typeof(TableStorage<>));

        return services;
    }
}
