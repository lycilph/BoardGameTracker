using BoardGameTracker.Application.Common.Notifications;
using BoardGameTracker.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BoardGameTracker.Client.Pages;

public partial class Home
{
    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;
    [Inject]
    public DummyService DummyService { get; set; } = null!;
    [Inject]
    public IPublisher Publisher { get; set; } = null!;

    private string time = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await UpdateTime();
    }

    private async Task UpdateTime()
    {
        try
        {
            time = await DummyService.GetTime();
        }
        catch (HttpRequestException e)
        {
            Logger.LogError("Error {error}", e.Message);
            time = e.Message;
        }

        StateHasChanged();
    }

    private async Task SendMessage()
    {
        await Publisher.Publish(new SnackbarNotification("Message from Home page"));
    }
}