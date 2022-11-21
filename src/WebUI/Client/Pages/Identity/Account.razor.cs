using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using BoardGameTracker.Application.Common.Extensions;

namespace BoardGameTracker.Client.Pages.Identity;

public partial class Account
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    private string username = string.Empty;
    private string bgg_username = string.Empty;
    private string email = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var auth_state = await AuthenticationStateTask;
        var user = auth_state.User;

        username = user.GetUsername() ?? string.Empty;
        bgg_username = user.GetBGGUsername() ?? string.Empty;
        email = user.GetEmail() ?? string.Empty;
    }
}