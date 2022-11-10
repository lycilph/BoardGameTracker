using MediatR;

namespace BoardGameTracker.Application.Common.Notifications;

public record class SnackbarNotification(string Message, bool Reauthenticate = false) : INotification;