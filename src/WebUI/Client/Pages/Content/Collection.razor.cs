using BoardGameTracker.Application.Common;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Game.Services;
using BoardGameTracker.Client.Extensions;
using BoardGameTracker.Client.Pages.Content.Models;
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

    private readonly LoadingState state;

    private List<BoardGameModel> games = new();
    private bool show_table = true;
    private string search_string = string.Empty;

    public Collection()
    {
        state = new LoadingState(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        using (state.Load())
        {
            var auth_state = await AuthenticationStateTask;
            var userid = auth_state.User.GetUserId()!;

            var temp = await Client.GetCollectionAsync(userid);
            games = temp.Select(BoardGameModel.Create).ToList();
        }
    }

    private bool FilterFunc(BoardGameModel game)
    {
        return search_string.IsNullOrWhiteSpace() ||
               game.Name.Contains(search_string, StringComparison.OrdinalIgnoreCase) ||
               game.YearPublished.Contains(search_string, StringComparison.OrdinalIgnoreCase);
    }

    private void RowClick(TableRowClickEventArgs<BoardGameModel> tableRowClickEventArgs)
    {
        Logger.LogInformation("Clicked {game}", tableRowClickEventArgs.Item.Name);
        DialogService.ShowGameDetails(tableRowClickEventArgs.Item.Game);
    }

    private void GameClick(BoardGameModel model)
    {
        Logger.LogInformation("Clicked {game}", model.Name);
        DialogService.ShowGameDetails(model.Game);
    }
}