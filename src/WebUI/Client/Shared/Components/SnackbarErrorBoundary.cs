using BoardGameTracker.Application.Common.Notifications;
using BoardGameTracker.Domain;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BoardGameTracker.Client.Shared.Components;

public class SnackbarErrorBoundary : ErrorBoundary
{
    [Inject]
    public ILogger<SnackbarErrorBoundary> Logger { get; set; } = null!;
    [Inject]
    public IPublisher Publisher { get; set; } = null!;

    protected override async Task OnErrorAsync(Exception exception)
    {
        if (exception is InvalidUserException user_exception)
            await Publisher.Publish(new SnackbarNotification(user_exception.Message, Reauthenticate: true));

        await base.OnErrorAsync(exception);
    }
}
