namespace BoardGameTracker.Domain.Data;

public class BoardGame
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string YearPublished { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public int NumberOfPlays { get; set; }
    public List<string> Status { get; set; } = new();
}
