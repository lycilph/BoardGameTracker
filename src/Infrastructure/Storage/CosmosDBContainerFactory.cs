using BoardGameTracker.Infrastructure.Config;
using BoardGameTracker.Infrastructure.Storage.Contracts;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace BoardGameTracker.Infrastructure.Storage;

public class CosmosDBContainerFactory : ICosmosDBContainerFactory
{
    private readonly CosmosDBSettings settings;
    private readonly ILogger<CosmosDBContainerFactory> logger;
    private readonly CosmosClient client;
    private Database database = null!;

    public CosmosDBContainerFactory(IOptions<CosmosDBSettings> options, ILogger<CosmosDBContainerFactory> logger)
    {
        settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var client_options = new CosmosClientOptions { AllowBulkExecution = true };
        client = new CosmosClient(settings.ConnectionString, client_options);
    }

    public async Task<Container> GetContainerAsync(string name, string partitionkey)
    {
        if (database == null)
        {
            var database_response = await client.CreateDatabaseIfNotExistsAsync(settings.DatabaseName, settings.Throughput);
            database = database_response.Database;

            if (database_response.StatusCode == HttpStatusCode.Created)
                logger.LogInformation("Initializing database {id} - {code}", database.Id, database_response.StatusCode);
        }

        var container_response = await database.CreateContainerIfNotExistsAsync(name, partitionkey);
        var container = container_response.Container;

        if (container_response.StatusCode == HttpStatusCode.Created)
            logger.LogInformation("Initializing container {id} - {code}", container.Id, container_response.StatusCode);

        return container;
    }
}
