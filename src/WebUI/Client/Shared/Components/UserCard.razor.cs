using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BoardGameTracker.Client.Shared.Components;

public partial class UserCard
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    [Parameter]
    public string Class { get; set; } = string.Empty;

    [Inject]
    public ILogger<UserCard> Logger { get; set; } = null!;

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
        }
    }

    private async Task LoadData()
    {
        var auth_state = await AuthenticationStateTask;
        var user = auth_state.User;

        Username = user.FindFirstValue(ClaimTypes.Name);
        Email = user.FindFirstValue(ClaimTypes.Email);

        StateHasChanged();
    }
}