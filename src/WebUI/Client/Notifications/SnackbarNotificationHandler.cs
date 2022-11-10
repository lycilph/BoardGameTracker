using BoardGameTracker.Application.Common.Notifications;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BoardGameTracker.Client.Notifications;

public class SnackbarNotificationHandler : INotificationHandler<SnackbarNotification>
{
    private readonly ISnackbar snackbar;
    private readonly NavigationManager navigation_manager;

    public SnackbarNotificationHandler(ISnackbar snackbar, NavigationManager navigation_manager)
    {
        this.snackbar = snackbar;
        this.navigation_manager = navigation_manager;
    }

#pragma warning disable IDE0039
    public Task Handle(SnackbarNotification notification, CancellationToken cancellationToken)
    {
        Action<SnackbarOptions> options = config =>
        {
            config.RequireInteraction = true;
            config.ShowCloseIcon = false;
            config.Onclick = snackbar => Logout();
        };

        var msg = notification.Message +
            (notification.Reauthenticate ?
            ". Click here to login again" :
            string.Empty);
        
        snackbar.Add(msg, Severity.Warning, notification.Reauthenticate ? options : null);

        return Task.CompletedTask;
    }

    private Task Logout()
    {
        navigation_manager.NavigateTo("/logout");
        return Task.CompletedTask;
    }
#pragma warning restore IDE0039
}