namespace AzureBlobLibrary;

public class ScopedBlobStorage : IScopedBlobStorage
{
    private readonly IBlobStorage _blobStorage;
    private readonly string _containerName;

    public ScopedBlobStorage(IBlobStorage blobStorage, string containerName)
    {
        _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
        _containerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
    }

    public Task<string> UploadAsync(string blobName, Stream content, string contentType = null, CancellationToken cancellationToken = default)
        => _blobStorage.UploadAsync(_containerName, blobName, content, contentType, cancellationToken);

    public Task<string> UploadAsync(string blobName, byte[] bytes, string contentType = null, CancellationToken cancellationToken = default)
        => _blobStorage.UploadAsync(_containerName, blobName, bytes, contentType, cancellationToken);

    public Task<Stream?> DownloadAsync(string blobName, CancellationToken cancellationToken = default)
        => _blobStorage.DownloadAsync(_containerName, blobName, cancellationToken);

    public Task<byte[]> DownloadBytesAsync(string blobName, CancellationToken cancellationToken = default)
        => _blobStorage.DownloadBytesAsync(_containerName, blobName, cancellationToken);

    public Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default)
        => _blobStorage.DeleteAsync(_containerName, blobName, cancellationToken);

    public Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default)
        => _blobStorage.ExistsAsync(_containerName, blobName, cancellationToken);

    public string GetBlobUrl(string blobName)
        => _blobStorage.GetBlobUrl(_containerName, blobName);

    public Task<List<string>> ListBlobsAsync(CancellationToken cancellationToken = default)
        => _blobStorage.ListBlobsAsync(_containerName, cancellationToken);
}
