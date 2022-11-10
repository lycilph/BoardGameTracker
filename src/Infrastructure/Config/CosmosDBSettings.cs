namespace BoardGameTracker.Infrastructure.Config;

public class CosmosDBSettings
{
    public const string Key = "CosmosDBSettings";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public int Throughput { get; set; } = 1000;
}
