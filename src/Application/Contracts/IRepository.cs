using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace BoardGameTracker.Application.Contracts;

public interface IRepository<TItem> where TItem : IItem
{
    Task<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TItem> GetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default);
    Task<TItem> GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TItem>> GetAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    Task<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default);
    Task UpdateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default);
}