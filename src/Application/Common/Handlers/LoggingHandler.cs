using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Application.Common.Handlers;

public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri;
        logger.LogDebug("HTTP Request: {uri} {@Request}", uri, request);

        return await base.SendAsync(request, cancellationToken);
    }
}
