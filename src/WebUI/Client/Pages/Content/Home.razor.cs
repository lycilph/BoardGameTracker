using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Game.Services;
using BoardGameTracker.Client.Extensions;
using BoardGameTracker.Domain.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BoardGameTracker.Client.Pages.Content;

public partial class Home
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;
    [Inject]
    public BoardGameGeekClient BGGClient { get; set; } = null!;
    [Inject]
    public GameClient GameClient { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    private List<BoardGame> hotness = new();
    private Profile profile = new();
    private bool loading = false;

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        var auth_state = await AuthenticationStateTask;
        var userid = auth_state.User.GetUserId()!;

        hotness = await BGGClient.GetHotnessAsync();
        StateHasChanged();

        profile = await GameClient.GetProfile(userid);

        loading = false;
        StateHasChanged();
    }

    private async Task GameClickAsync(BoardGame game)
    {
        Logger.LogInformation("Clicked {game}", game.Name);

        game = await BGGClient.GetGameDetails(game.Id);
        DialogService.ShowGameDetails(game);
    }
}