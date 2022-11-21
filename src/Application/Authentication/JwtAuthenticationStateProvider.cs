using BoardGameTracker.Application.Authentication.Data;
using BoardGameTracker.Application.Authentication.Storage;
using BoardGameTracker.Application.Common.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BoardGameTracker.Application.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStore token_store;
    private readonly AuthenticationState anonymous;

    public JwtAuthenticationStateProvider(ITokenStore token_store)
    {
        this.token_store = token_store;

        anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await token_store.GetTokenAsync();
        if (token.IsNullOrWhiteSpace())
            return anonymous;

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), AuthenticationTypeConstant.JwtAutenticationType)));
    }

    public async Task NotifyUserAuthentication()
    {
        var token = await token_store.GetTokenAsync();
        var claims = JwtParser.ParseClaimsFromJwt(token);
        var authenticated_user = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationTypeConstant.JwtAutenticationType));
        var state = Task.FromResult(new AuthenticationState(authenticated_user));
        NotifyAuthenticationStateChanged(state);
    }

    public void NotifyUserLogout()
    {
        var state = Task.FromResult(anonymous);
        NotifyAuthenticationStateChanged(state);
    }
}
