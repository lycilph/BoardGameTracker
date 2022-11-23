using BoardGameTracker.Application.Authentication.Data;
using BoardGameTracker.Application.Services.Mail;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Application.Contracts;

namespace BoardGameTracker.Infrastructure.Mail;

public class MailService : IMailService
{
    private readonly IIdentityService identity_service;
    private readonly IMailSender mail_sender;

    public MailService(IIdentityService identity_service, IMailSender mail_sender)
    {
        this.identity_service = identity_service;
        this.mail_sender = mail_sender;
    }

    public async Task SendConfirmationEmail(ApplicationUser user, string origin)
    {
        var code = await identity_service.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var uri = new Uri($"{origin}/api/authentication/confirmemail/");
        var url = QueryHelpers.AddQueryString(uri.ToString(), "userid", user.Id);
        url = QueryHelpers.AddQueryString(url, "code", code);

        var message = $"Please confirm you email by <a href='{HtmlEncoder.Default.Encode(url)}'>clicking here</a>";
        await mail_sender.SendEmailAsync(user.Email!, "Confirm email", message);
    }
}
