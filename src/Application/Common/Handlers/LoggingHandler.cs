using Microsoft.Extensions.Logging;
using System.Diagnostics;

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
        var sw = new Stopwatch();
        HttpResponseMessage response;
        using (new Benchmark(sw))
        { 
            response = await base.SendAsync(request, cancellationToken);
        }

        var uri = request.RequestUri;
        logger.LogInformation("HTTP (client) {method} {uri} responded {code} in {time} ms", request.Method, uri, response.StatusCode, sw.ElapsedMilliseconds);

        return response;
    }
}
