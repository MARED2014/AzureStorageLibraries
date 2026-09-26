namespace AzureBlobLibrary;

public interface IScopedBlobStorage
{
    Task<string> UploadAsync(string blobName, Stream content, string contentType = null, CancellationToken cancellationToken = default);
    Task<string> UploadAsync(string blobName, byte[] bytes, string contentType = null, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string blobName, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadBytesAsync(string blobName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default);
    string GetBlobUrl(string blobName);

    Task<List<string>> ListBlobsAsync(string containerName, CancellationToken cancellationToken = default);
}
