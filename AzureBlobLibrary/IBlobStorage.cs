namespace AzureBlobLibrary;

public interface IBlobStorage
{
    Task<string> UploadAsync(string containerName, string blobName, Stream content, string contentType = null, CancellationToken cancellationToken = default);
    Task<string> UploadAsync(string containerName, string blobName, byte[] bytes, string contentType = null, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadBytesAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
    string GetBlobUrl(string containerName, string blobName);
    Task<List<string>> ListBlobsAsync(string containerName, CancellationToken cancellationToken = default);
}
