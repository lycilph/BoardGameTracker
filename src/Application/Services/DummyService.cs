using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Application.Services;

public class DummyService
{
    private readonly HttpClient client;
    private readonly ILogger<DummyService> logger;

    public DummyService(HttpClient client, ILogger<DummyService> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task<string> GetTime()
    {
        logger.LogInformation("Getting time now");
        var response = await client.GetStringAsync("dummy/time");
        return response;
    }
}
