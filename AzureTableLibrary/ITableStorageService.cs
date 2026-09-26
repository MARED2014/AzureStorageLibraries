using Azure.Data.Tables;
using System.Linq.Expressions;

namespace AzureTableLibrary;

public interface ITableStorageService<T> where T : class, ITableEntity, new()
{
    Task<T> Add(T entity, CancellationToken cancellationToken = default);
    Task Delete(string partitionKey, string rowKey, CancellationToken cancellationToken = default);
    Task<T> Update(T entity, CancellationToken cancellationToken = default);
    Task<T?> Get(string partitionKey, string rowKey, CancellationToken cancellationToken = default);
    IQueryable<T> GetAll();
    IQueryable<T> Query(Expression<Func<T, bool>> predicate);
    IAsyncEnumerable<T> QueryAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
