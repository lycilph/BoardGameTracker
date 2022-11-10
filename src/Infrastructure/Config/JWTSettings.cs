namespace BoardGameTracker.Infrastructure.Config;

public class JWTSettings
{
    public const string Key = "JWTSettings";

    public string SecurityKey { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; }
}
