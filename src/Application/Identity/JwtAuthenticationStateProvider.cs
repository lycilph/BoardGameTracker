using BoardGameTracker.Application.Identity.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BoardGameTracker.Application.Identity;

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
        if (string.IsNullOrWhiteSpace(token))
            return anonymous;

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = JwtParser.ParseClaimsFromJwt(token);
        var authenticated_user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwtAuthType"));
        var state = Task.FromResult(new AuthenticationState(authenticated_user));
        NotifyAuthenticationStateChanged(state);
    }

    public void NotifyUserLogout()
    {
        var state = Task.FromResult(anonymous);
        NotifyAuthenticationStateChanged(state);
    }
}
