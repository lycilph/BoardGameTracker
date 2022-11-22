using BoardGameTracker.Application.Authentication.DTO;
using BoardGameTracker.Application.Authentication.Services;
using BoardGameTracker.Application.Authentication.Storage;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Refit;

namespace BoardGameTracker.Infrastructure.Authentication.Services;

public class AuthenticationClient : IAuthenticationClient
{
    private readonly IAuthenticationClientInternal client;
    private readonly ITokenStore token_store;
    private readonly JwtAuthenticationStateProvider auth_provider;
    private readonly ILogger<AuthenticationClient> logger;

    public AuthenticationClient(
        IAuthenticationClientInternal client,
        ITokenStore token_store,
        AuthenticationStateProvider auth_provider,
        ILogger<AuthenticationClient> logger)
    {
        this.client = client;
        this.token_store = token_store;
        this.auth_provider = auth_provider as JwtAuthenticationStateProvider ?? throw new ArgumentException(nameof(AuthenticationStateProvider));
        this.logger = logger;
    }

    public async Task<IApiResponse<AuthenticationResponse>> Login(LoginRequest request)
    {
        var response = await client.Login(request);

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content!;
            await token_store.SetTokensAsync(content.Token, content.RefreshToken, request.RememberMe);
            await auth_provider.NotifyUserAuthentication();
        }

        return response;
    }

    public async Task<IApiResponse<AuthenticationResponse>> RegisterUser(RegisterRequest request)
    {
        return await client.RegisterUser(request);
    }

    public async Task<IApiResponse<AuthenticationResponse>> RefreshToken()
    {
        (var token, var refresh_token, var is_persisted) = await token_store.GetTokensAsync();
        var request = new RefreshTokenRequest { Token = token, RefreshToken = refresh_token };
        var response = await client.RefreshToken(request);

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content!;
            await token_store.SetTokensAsync(content.Token, content.RefreshToken, is_persisted);
        }
        else
        {
            if (response.Content!.Error.IsNullOrWhiteSpace())
                logger.LogError("{errors}", string.Join(",", response.Content.Errors));
            else
                logger.LogError("{error}", response.Content.Error);
        }

        return response;
    }

    public async Task Logout()
    {
        await token_store.RemoveTokensAsync();
        auth_provider.NotifyUserLogout();
    }
}
