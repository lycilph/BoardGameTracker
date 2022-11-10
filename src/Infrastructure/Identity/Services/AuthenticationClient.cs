using BoardGameTracker.Application.Identity;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Identity.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;

namespace BoardGameTracker.Infrastructure.Identity.Services;

public class AuthenticationClient : IAuthenticationClient
{
    private readonly IAuthenticationClientInternal client;
    private readonly ITokenStore token_store;
    private readonly ApplicationAuthenticationStateProvider auth_provider;

    public AuthenticationClient(IAuthenticationClientInternal client, ITokenStore token_store, AuthenticationStateProvider auth_provider)
    {
        this.client = client;
        this.token_store = token_store;
        this.auth_provider = auth_provider as ApplicationAuthenticationStateProvider ?? throw new ArgumentException(nameof(AuthenticationStateProvider));
    }

    public async Task<IApiResponse<AuthenticationResponse>> Login(LoginRequest request)
    {
        var response = await client.Login(request);

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content!;
            await token_store.SetTokensAsync(content.Token, content.RefreshToken, request.RememberMe);
            auth_provider.NotifyUserAuthentication(content.Token);
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
     
        return response;
    }

    public async Task Logout()
    {
        await token_store.RemoveTokensAsync();
        auth_provider.NotifyUserLogout();
    }
}
