using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Domain.Data;
using Microsoft.AspNetCore.Components;

namespace BoardGameTracker.Client.Pages.Content;

public partial class Home
{
    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;
    [Inject]
    public BoardGameGeekClient Client { get; set; } = null!;

    private List<BoardGame> games = new();

    protected override async Task OnInitializedAsync()
    {
        games = await Client.GetHotnessAsync();
        StateHasChanged();
    }

    private void GameClick(BoardGame game)
    {
        Logger.LogInformation($"Clicked {game.Name}");
    }
}