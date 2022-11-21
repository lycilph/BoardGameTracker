using Microsoft.Azure.Cosmos;

namespace BoardGameTracker.Infrastructure.Storage.Contracts;

public interface ICosmosDBContainerFactory
{
    Task<Container> GetContainerAsync(string name, string partitionkey);
}