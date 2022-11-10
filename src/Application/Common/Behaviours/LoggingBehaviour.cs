using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest>
    : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger logger;

    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        this.logger = logger;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogDebug("MediatR Request: {Name} {@Request}", requestName, request);
        return Task.CompletedTask;
    }
}
