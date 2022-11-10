using Microsoft.Azure.Cosmos;

namespace BoardGameTracker.Infrastructure.Contracts;

public interface ICosmosDBContainerFactory
{
    Task<Container> GetContainerAsync(string name, string partitionkey);
}