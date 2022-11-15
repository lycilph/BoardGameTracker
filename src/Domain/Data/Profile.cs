namespace BoardGameTracker.Domain.Data;

public class Profile
{
    public string Id { get; set; } = string.Empty;
    public string LastUsedBGGUsername { get; set; } = string.Empty; //Used when interacting with the BGG API
}
