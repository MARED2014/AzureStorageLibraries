using Azure;
using Azure.Data.Tables;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace AzureTableLibrary;

public class TableStorage<TEntity> : ITableStorageService<TEntity> where TEntity : class, ITableEntity, new()
{
    private readonly TableClient _table;
    public TableStorage(TableServiceClient serviceClient)
    {
        ArgumentNullException.ThrowIfNull(serviceClient);

        _table = serviceClient.GetTableClient(typeof(TEntity).Name);
        _table.CreateIfNotExists();
    }

    public async Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _table.AddEntityAsync(entity, cancellationToken);
        return entity;
    }

    public async Task Delete(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
    {
        await _table.DeleteEntityAsync(partitionKey, rowKey, ETag.All, cancellationToken);
    }

    public async Task<TEntity?> Get(string partitionKey, string rowKey, CancellationToken cancellationToken = default)
    {
        Response<TEntity> response = await _table.GetEntityAsync<TEntity>(partitionKey, rowKey, cancellationToken: cancellationToken);
        return response.Value;
    }

    public IQueryable<TEntity> GetAll()
    {
        return _table.Query<TEntity>().AsQueryable();
    }

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
    {
        return _table.Query(predicate).AsQueryable();
    }

    public async IAsyncEnumerable<TEntity> QueryAsync(Expression<Func<TEntity, bool>> predicate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        AsyncPageable<TEntity> pageableResult = _table.QueryAsync(predicate, cancellationToken: cancellationToken);

        await foreach (TEntity item in pageableResult.WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }

    public async Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _table.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace, cancellationToken);
        return entity;
    }
}