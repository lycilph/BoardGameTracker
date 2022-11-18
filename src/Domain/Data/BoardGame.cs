namespace BoardGameTracker.Domain.Data;

public class BoardGame
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string YearPublished { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int Playingtime { get; set; } 
    public int MinPlaytime { get; set; }
    public int MaxPlaytime { get; set; }
    public int MinAge { get; set; }
    public string Rating { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NumberOfPlays { get; set; }
    public List<string> Status { get; set; } = new();
}
