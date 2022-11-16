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
}