using BoardGameTracker.Application.Authentication;
using BoardGameTracker.Application.Authentication.Services;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameTracker.Infrastructure.Identity.Services;

public class IdentityClient : IIdentityClient
{
    private readonly HttpClient client;
    private readonly IAuthenticationClient authentication_client;
    private readonly JwtAuthenticationStateProvider auth_provider;
    private readonly NavigationManager navigation_manager;

    public IdentityClient(HttpClient client, IAuthenticationClient authentication_client, AuthenticationStateProvider auth_provider, NavigationManager navigation_manager)
    {
        this.client = client;
        this.authentication_client = authentication_client;
        this.auth_provider = auth_provider as JwtAuthenticationStateProvider ?? throw new ArgumentException(nameof(AuthenticationStateProvider));
        this.navigation_manager = navigation_manager;
    }

    public async Task DeleteAccount(string userid)
    {
        var response = await client.DeleteAsync($"DeleteAccount/{userid}");
        if (response.IsSuccessStatusCode)
        {
            navigation_manager.NavigateTo("/authentication/logout");
        }
        else
        {
            var msg = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(msg);
        }
    }

    public async Task UpdateBGGUsername(string userid, string bgg_username)
    {
        var update_request = new UpdateBGGUsernameRequest { UserId = userid, BGGUsername = bgg_username };
        var response = await client.PatchAsJsonAsync("UpdateBGGUsername", update_request);

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

    public async Task<UpdateEmailResponse> UpdateEmail(UpdateEmailRequest request)
    {
        var response = await client.PostAsJsonAsync("UpdateEmail", request);

        // If code is NoContent, this is a no-op from the controller, and the tokens doesn't need to be refreshed
        if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.NoContent)
            return UpdateEmailResponse.NoOp();

        // The tokens needs to be updated here, as the email claim is not updated otherwise
        if (response.IsSuccessStatusCode)
        {
            await authentication_client.RefreshToken();
            await auth_provider.NotifyUserAuthentication();
        }

        return await response.Content.ReadFromJsonAsync<UpdateEmailResponse>() ?? UpdateEmailResponse.NoOp();
    }

    public async Task<UpdatePasswordResponse> UpdatePassword(UpdatePasswordRequest request)
    {
        var response = await client.PostAsJsonAsync("UpdatePassword", request);
        return await response.Content.ReadFromJsonAsync<UpdatePasswordResponse>() ?? UpdatePasswordResponse.Failure("Unknown error");
    }

    public async Task<GetUserInfoResponse> GetUserInfo(string userid)
    {
        try 
        {
            return await client.GetFromJsonAsync<GetUserInfoResponse>($"GetUserInfo/{userid}") ?? GetUserInfoResponse.Failure("Couldn't get user info");
        }
        catch (Exception e)
        {
            return GetUserInfoResponse.Failure($"Error getting user info {e.Message}");
        }
    }
}
