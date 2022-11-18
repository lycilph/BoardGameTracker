namespace BoardGameTracker.Infrastructure.Config;

public class MailOptions
{
    public const string Key = "MailOptions";

    public string Name { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
