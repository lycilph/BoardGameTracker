using BoardGameTracker.Client.Shared.Components;
using BoardGameTracker.Domain.Data;
using MudBlazor;

namespace BoardGameTracker.Client.Extensions;

public static class DialogServiceExtensions
{
    public static void ShowGameDetails(this IDialogService dialog_service, BoardGame game)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            NoHeader = true,
            MaxWidth = MaxWidth.Medium
        };
        var parameters = new DialogParameters { ["Game"] = game };

        dialog_service.Show<GameDetails>("Game Details", parameters, options);
    }
}
