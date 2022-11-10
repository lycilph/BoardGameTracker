namespace BoardGameTracker.Infrastructure.Config;

public class IdentitySeedData
{
    public const string Key = "IdentitySeedData";

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
