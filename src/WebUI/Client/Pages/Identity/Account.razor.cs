using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Identity.Services;

namespace BoardGameTracker.Client.Pages.Identity;

public partial class Account
{
    private string userid = string.Empty;
    private string username = string.Empty;
    private string bgg_username = string.Empty;
    private string email = string.Empty;
    private string account_created = string.Empty;
    private string last_active = string.Empty;
    private bool loading = false;
    private bool card_loading = false;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Inject]
    public IIdentityClient Client { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var auth_state = await AuthenticationStateTask;
        var user = auth_state.User;

        userid = user.GetUserId()!;
        username = user.GetUsername() ?? string.Empty;
        bgg_username = user.GetBGGUsername() ?? string.Empty;
        email = user.GetEmail() ?? string.Empty;

        if (account_created.IsNullOrWhiteSpace() && !userid.IsNullOrWhiteSpace())
        {
            card_loading = true;
            
            var info = await Client.GetUserInfo(userid);
            account_created = info.AccountCreated.ToShortDateString();
            last_active = $"{info.LastActive.ToShortDateString()} - {info.LastActive.ToShortTimeString()}";

            card_loading = false;
        }
    }

    private void OnLoadingChanged(bool is_loading)
    {
        loading = is_loading;
    }
}