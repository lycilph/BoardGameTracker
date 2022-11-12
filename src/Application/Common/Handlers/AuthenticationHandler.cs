using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Identity.Storage;
using BoardGameTracker.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;

namespace BoardGameTracker.Application.Common.Handlers;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly ITokenStore token_store;
    private readonly IAuthenticationClient client;
    private readonly ILogger<AuthenticationHandler> logger;

    public AuthenticationHandler(ITokenStore token_store, IAuthenticationClient client, ILogger<AuthenticationHandler> logger)
    {
        this.token_store = token_store;
        this.client = client;
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await token_store.GetTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            logger.LogInformation("Refreshing token now");
            var refresh_response = await client.RefreshToken();

            if (refresh_response.IsSuccessStatusCode)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", refresh_response.Content!.Token);
                response = await base.SendAsync(request, cancellationToken);
            }
            else
            {
                throw new InvalidUserException(refresh_response.Error!.Content!);
            }
        }

        return response;
    }
}
