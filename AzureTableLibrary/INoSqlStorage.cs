using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AzureTableLibrary;

public interface INoSqlStorage<T>
{
    Task<T> Add(T entity);
    Task Delete(string partitionKey, string rowKey);
    Task<T> Update(T entity);
    Task<T> Get(string partitionKey, string rowKey);
    IQueryable<T> GetAll();
    IQueryable<T> Query(Expression<Func<T, bool>> predicate);
}
