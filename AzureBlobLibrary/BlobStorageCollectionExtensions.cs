using Microsoft.Extensions.DependencyInjection;

namespace AzureBlobLibrary;

public static class BlobStorageCollectionExtensions
{
    /// <summary>
    /// Azure Blob Storage servislerini string anahtarlar (Keyed Services) kullanarak DI konteynerine ekler.
    /// Container isimleri ve DI anahtarları doğrudan metin (string) olarak tanımlanır.
    /// </summary>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="configureContainers">Key/ContainerName eşleşmelerinin yapılacağı konfigürasyon eylemi.</param>
    /// <returns>Servis koleksiyonunun kendisi.</returns>
    /// <example>
    /// <code>
    /// // 1. Program.cs (DI Kaydı):
    /// builder.Services.AddSingleton(new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));
    /// 
    /// builder.Services.AddAzureBlobStorage(containers =>
    /// {
    ///     containers.Add("UserProfiles", "user-profiles");
    ///     containers.Add("Invoices", "invoices");
    /// });
    ///
    /// // 2. Servis/Sınıf İçinde Kullanımı:
    /// public class UserService
    /// {
    ///     private readonly IScopedBlobStorage _profileStorage;
    ///
    ///     public UserService([FromKeyedServices("UserProfiles")] IScopedBlobStorage profileStorage)
    ///     {
    ///         _profileStorage = profileStorage;
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, Action<Dictionary<string, string>> configureContainers)
    {
        services.AddSingleton<IBlobStorage, BlobStorageService>();
        var containerMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        configureContainers(containerMap);

        foreach (var (key, containerName) in containerMap)
        {
            services.AddKeyedTransient<IScopedBlobStorage>(key, (sp, requiredKey) =>
            {
                var baseStorage = sp.GetRequiredService<IBlobStorage>();
                return new ScopedBlobStorage(baseStorage, containerName);
            });
        }

        return services;
    }


    /// <summary>
    /// Azure Blob Storage servislerini projenize ait bir Enum türü üzerinden Tip-Güvenli (Type-Safe) 
    /// Keyed Services olarak DI konteynerine otomatik kaydeder.
    /// </summary>
    /// <typeparam name="TEnum">Container anahtarlarını temsil eden Enum türü.</typeparam>
    /// <param name="services">Servis koleksiyonu.</param>
    /// <param name="containerNameResolver">
    /// Enum değerini Azure Storage kurallarına uygun container ismine çeviren özelleştirilmiş çözücü fonksiyon. 
    /// <para>Belirtilmezse varsayılan olarak enum adını küçük harfe (<c>ToLowerInvariant</c>) çevirir.</para>
    /// </param>
    /// <returns>Servis koleksiyonunun kendisi.</returns>
    /// <example>
    /// <code>
    /// // Web Projesindeki Enum:
    /// public enum StorageContainerKey { UserProfiles, Invoices }
    ///
    /// // 1. Program.cs (DI Kaydı):
    /// builder.Services.AddAzureBlobStorage<StorageContainerKey>(key => key switch
    /// {
    ///     StorageContainerKey.UserProfiles => "user-profiles",
    ///     StorageContainerKey.Invoices => "invoices",
    ///     _ => key.ToString().ToLowerInvariant()
    /// });
    ///
    /// // 2. Servis/Sınıf İçinde Kullanımı:
    /// public class UserService
    /// {
    ///     private readonly IScopedBlobStorage _profileStorage;
    ///
    ///     public UserService([FromKeyedServices(StorageContainerKey.UserProfiles)] IScopedBlobStorage profileStorage)
    ///     {
    ///         _profileStorage = profileStorage;
    ///     }
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddAzureBlobStorage<TEnum>(this IServiceCollection services, Func<TEnum, string>? containerNameResolver = null) where TEnum : struct, Enum
    {
        services.AddSingleton<IBlobStorage, BlobStorageService>();

        foreach (TEnum enumValue in Enum.GetValues<TEnum>())
        {
            object key = enumValue;
            string containerName = containerNameResolver != null ? containerNameResolver(enumValue) : enumValue.ToString().ToLowerInvariant();

            services.AddKeyedTransient<IScopedBlobStorage>(key, (sp, requiredKey) =>
            {
                var baseStorage = sp.GetRequiredService<IBlobStorage>();
                return new ScopedBlobStorage(baseStorage, containerName);
            });
        }

        return services;
    }
}
