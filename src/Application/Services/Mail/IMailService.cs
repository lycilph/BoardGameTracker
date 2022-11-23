using BoardGameTracker.Application.Authentication.Data;

namespace BoardGameTracker.Application.Services.Mail;

public interface IMailService
{
    Task SendConfirmationEmail(ApplicationUser user, string origin);
}
