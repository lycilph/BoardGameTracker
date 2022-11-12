using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoardGameTracker.Client.Pages.Content;

public partial class Home
{
    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;
    [Inject]
    public BoardGameGeekClient Client { get; set; } = null!;

    private MudCarousel<object>? carousel;
    private List<BoardGame> games = new();
    private bool is_loading = false;

    protected override async Task OnInitializedAsync()
    {
        is_loading = true;
        
        var temp = await Client.GetHotnessAsync();
        games = temp.ToList();

        is_loading = false;
        carousel?.MoveTo(0);

        StateHasChanged();
    }
}