using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BoardGameTracker.Application.Common.Extensions;

namespace BoardGameTracker.Client.Pages.Identity;

public partial class Account
{
    private string username = string.Empty;
    private string bgg_username = string.Empty;
    private string email = string.Empty;
    private bool loading = false;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var auth_state = await AuthenticationStateTask;
        var user = auth_state.User;

        username = user.GetUsername() ?? string.Empty;
        bgg_username = user.GetBGGUsername() ?? string.Empty;
        email = user.GetEmail() ?? string.Empty;
    }

    private void OnLoadingChanged(bool is_loading)
    {
        loading = is_loading;
    }
}