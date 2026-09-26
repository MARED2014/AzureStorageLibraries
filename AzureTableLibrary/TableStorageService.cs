using Azure.Data.Tables;
using SharedLayer;
using System.Linq.Expressions;

namespace AzureTableLibrary;

public class TableStorage<TEntity> : INoSqlStorage<TEntity> where TEntity : class, ITableEntity, new()
{
    private readonly TableClient _table;

    public TableStorage()
    {
        TableServiceClient _tableServiceClient = new TableServiceClient(ConnectionStrings.AzureStorageConnectionString);
        _table = _tableServiceClient.GetTableClient(typeof(TEntity).Name);
        _table.CreateIfNotExists();
    }

    public async Task<TEntity> Add(TEntity entity)
    {
        await _table.AddEntityAsync(entity);
        return entity;
    }

    public async Task Delete(string partitionKey, string rowKey)
    {
        await _table.DeleteEntityAsync(partitionKey, rowKey);
    }

    public async Task<TEntity> Get(string partitionKey, string rowKey)
    {
        var entity = await _table.GetEntityAsync<TEntity>(partitionKey, rowKey);
        return entity;
    }

    public IQueryable<TEntity> GetAll()
    {
        IQueryable<TEntity> queryable = _table.Query<TEntity>().AsQueryable();
        return queryable;
    }

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
    {
        return _table.Query(predicate).AsQueryable();
    }

    public async Task<TEntity> Update(TEntity entity)
    {
        await _table.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace);
        return entity;
    }
}
