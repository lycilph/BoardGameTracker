using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Identity.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BoardGameTracker.Client.Pages.Identity;

public partial class DeleteAccount
{
    private string userid = string.Empty;

    private bool show_dialog = false;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Parameter]
    public EventCallback<bool> IsLoading { get; set; }
    
    [Inject]
    public IIdentityClient Client { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var auth_state = await AuthenticationStateTask;
        var user = auth_state.User;

        userid = user.GetUserId()!;
    }

    private async Task OnLoadingChangedHandlerAsync(bool is_loading)
    {
        await IsLoading.InvokeAsync(is_loading);
    }
    
    private async void Delete()
    {
        await OnLoadingChangedHandlerAsync(true);

        show_dialog = true;
    }

    private async Task CancelAsync()
    {
        show_dialog = false;
        await OnLoadingChangedHandlerAsync(false);
    }

    private async Task OkAsync()
    {
        show_dialog = false;
        await Client.DeleteAccount(userid);
        await OnLoadingChangedHandlerAsync(false);
    }
}