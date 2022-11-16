namespace BoardGameTracker.Domain.Data;

public class Profile
{
    public string Id { get; set; } = string.Empty;
    public List<BoardGame> Games { get; set; } = new();
    public List<Play> Plays { get; set; } = new();
}
