using BoardGameTracker.Application.Authentication;
using BoardGameTracker.Application.Authentication.Services;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameTracker.Infrastructure.Identity.Services;

public class IdentityClient : IIdentityClient
{
    private readonly HttpClient client;
    private readonly IAuthenticationClient authentication_client;
    private readonly JwtAuthenticationStateProvider auth_provider;

    public IdentityClient(HttpClient client, IAuthenticationClient authentication_client, AuthenticationStateProvider auth_provider)
    {
        this.client = client;
        this.authentication_client = authentication_client;
        this.auth_provider = auth_provider as JwtAuthenticationStateProvider ?? throw new ArgumentException(nameof(AuthenticationStateProvider));
    }

    public async Task UpdateBGGUsername(string userid, string bgg_username)
    {
        var update_request = new UpdateBGGUsernameRequest { UserId = userid, BGGUsername = bgg_username };
        var request = new HttpRequestMessage(HttpMethod.Patch, "UpdateBGGUsername")
        {
            Content = JsonContent.Create(update_request)
        };

        var response = await client.SendAsync(request);

        // If code is NoContent, this is a no-op from the controller, and the tokens doesn't need to be refreshed
        if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
        {
            // The tokens needs to be updated here, as the bgg username claim is not there otherwise
            await authentication_client.RefreshToken();
            await auth_provider.NotifyUserAuthentication();
        }
    }

    public async Task<UpdateAccountResponse> UpdateAccount(UpdateAccountRequest request)
    {
        var response = await client.PostAsJsonAsync("UpdateAccount", request);

        // If code is NoContent, this is a no-op from the controller, and the tokens doesn't need to be refreshed
        if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.NoContent)
            return UpdateAccountResponse.NoOp();
        
        // The tokens needs to be updated here, as the BGGUsername or Username claim is not updated otherwise
        if (response.IsSuccessStatusCode)
        {
            await authentication_client.RefreshToken();
            await auth_provider.NotifyUserAuthentication();
        }

        return await response.Content.ReadFromJsonAsync<UpdateAccountResponse>() ?? UpdateAccountResponse.NoOp();
    }
}
