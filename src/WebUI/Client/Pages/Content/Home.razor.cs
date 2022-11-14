using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Data;
using Microsoft.AspNetCore.Components;

namespace BoardGameTracker.Client.Pages.Content;

public partial class Home
{
    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;
    [Inject]
    public BoardGameGeekClient Client { get; set; } = null!;

    private List<BoardGame> games = new();
    private bool is_loading = false;

    protected override async Task OnInitializedAsync()
    {
        is_loading = true;
        games = await Client.GetHotnessAsync();
        is_loading = false;
        
        StateHasChanged();
    }

    private void GameClick(BoardGame game)
    {
        Logger.LogInformation($"Clicked {game.Name}");
    }
}