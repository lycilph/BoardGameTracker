using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Game.Services;
using BoardGameTracker.Client.Extensions;
using BoardGameTracker.Domain.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BoardGameTracker.Client.Pages.Content;

public partial class Collection
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;
    [Inject]
    public GameClient Client { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    private List<BoardGame> games = new();
    private bool is_loading = false;
    private bool show_table = true;
    private string search_string = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        is_loading = true;

        var auth_state = await AuthenticationStateTask;
        var userid = auth_state.User.GetUserId()!;

        games = await Client.GetCollectionAsync(userid);

        is_loading= false;
        StateHasChanged();
    }

    private bool FilterFunc(BoardGame game)
    {
        return search_string.IsNullOrWhiteSpace() ||
               game.Name.Contains(search_string, StringComparison.OrdinalIgnoreCase) ||
               game.YearPublished.Contains(search_string, StringComparison.OrdinalIgnoreCase);
    }

    private void RowClick(TableRowClickEventArgs<BoardGame> tableRowClickEventArgs)
    {
        Logger.LogInformation("Clicked {game}", tableRowClickEventArgs.Item.Name);
        DialogService.ShowGameDetails(tableRowClickEventArgs.Item);
    }

    private void GameClick(BoardGame game)
    {
        Logger.LogInformation("Clicked {game}", game.Name);
        DialogService.ShowGameDetails(game);
    }
}