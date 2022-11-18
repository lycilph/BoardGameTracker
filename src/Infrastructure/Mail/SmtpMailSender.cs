using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Infrastructure.Config;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BoardGameTracker.Infrastructure.Mail;

public class SmtpMailSender : IMailSender
{
    private readonly MailOptions options;
    private readonly ILogger<SmtpMailSender> logger;

    public SmtpMailSender(IOptions<MailOptions> options, ILogger<SmtpMailSender> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.Name, options.Username));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlMessage };

        try
        {
            using (var client = new SmtpClient())
            {
                client.Connect(options.Host, options.Port, false);
                if (!options.Password.IsNullOrWhiteSpace())
                    client.Authenticate(options.Username, options.Password);
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, ex.Message);
        }
    }
}