using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Domain.Data;
using Microsoft.AspNetCore.Components;

namespace BoardGameTracker.Client.Pages.Content;

public partial class Collection
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
        await Task.Delay(1000);
        is_loading= false;
    }
}