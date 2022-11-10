using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Infrastructure.Contracts;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;
using System.Net;

namespace BoardGameTracker.Infrastructure.Storage;

public class Repository<TItem> : IRepository<TItem> where TItem : IItem
{
    private readonly Container container;

    public Repository(ICosmosDBContainerFactory container_factory)
    {
        var attribute = Attribute.GetCustomAttribute(typeof(TItem), typeof(ContainerInfoAttribute));
        var info = attribute as ContainerInfoAttribute ?? throw new ArgumentException(nameof(ContainerInfoAttribute));

        container = container_factory.GetContainerAsync(info.Name, info.PartitionKey).Result;
    }

    public async Task<TItem> CreateAsync(TItem value, CancellationToken cancellationToken = default)
    {
        ItemResponse<TItem> response = await container.CreateItemAsync(value, new PartitionKey(value.PartitionKey), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public Task<bool> ExistsAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default) =>
        ExistsAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

    public async Task<bool> ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        if (partitionKey == default)
            partitionKey = new PartitionKey(id);

        try
        {
            _ = await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
    {
        IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>().Where(predicate);

        int count = await query.CountAsync(cancellationToken);
        return count > 0;
    }

    public Task<TItem> GetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default) =>
        GetAsync(id, new PartitionKey(partitionKeyValue ?? id), cancellationToken);

    public async Task<TItem> GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = default)
    {
        if (partitionKey == default)
            partitionKey = new PartitionKey(id);

        ItemResponse<TItem> response =
            await container.ReadItemAsync<TItem>(id, partitionKey, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.Resource;
    }

    public async Task<IEnumerable<TItem>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
    {
        IQueryable<TItem> query = container.GetItemLinqQueryable<TItem>().Where(predicate);

        List<TItem> results = new();
        using (var feed = query.ToFeedIterator())
        {
            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync(cancellationToken).ConfigureAwait(false);
                foreach (var result in response.Resource)
                    results.Add(result);
            }
        }
        return results;
    }

    public async Task<IEnumerable<TItem>> GetAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var tasks = ids.Select(id => GetAsync(id, cancellationToken: cancellationToken)).ToList();
        _ = await Task.WhenAll(tasks).ConfigureAwait(false);
        return tasks.Select(t => t.Result);
    }

    public async Task<TItem> UpdateAsync(TItem value, CancellationToken cancellationToken = default)
    {
        var response =
            await container.UpsertItemAsync(value, new PartitionKey(value.PartitionKey), null,
                    cancellationToken)
                .ConfigureAwait(false);

        return response.Resource;
    }
}
