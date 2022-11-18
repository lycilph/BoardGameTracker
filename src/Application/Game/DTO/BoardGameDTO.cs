using BoardGameTracker.Application.Contracts;
using Newtonsoft.Json;

namespace BoardGameTracker.Application.Game.DTO;

[ContainerInfo("Games", "/id")]
public class BoardGameDTO : IItem
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonProperty("type")]
    public string Type { get; set; } = nameof(BoardGameDTO);

    string IItem.PartitionKey => GetPartitionKeyValue();
    protected virtual string GetPartitionKeyValue() => Id;

    public string Name { get; set; } = string.Empty;
    public string YearPublished { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int PlayingTime { get; set; }
    public int MinPlaytime { get; set; }
    public int MaxPlaytime { get; set; }
    public int MinAge { get; set; }
    public string Rating { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}