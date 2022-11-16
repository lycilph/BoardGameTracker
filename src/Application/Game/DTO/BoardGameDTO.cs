using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Domain.Data;
using Newtonsoft.Json;

namespace BoardGameTracker.Application.Game.DTO;

[ContainerInfo("Games", "/id")]
public class BoardGameDTO : BoardGame, IItem
{
    [JsonProperty("id")]
    public new string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonProperty("type")]
    public string Type { get; set; } = nameof(ProfileDTO);

    string IItem.PartitionKey => GetPartitionKeyValue();
    protected virtual string GetPartitionKeyValue() => Id;
}