using BoardGameTracker.Application.Contracts;
using Newtonsoft.Json;

namespace BoardGameTracker.Application.Game.DTO;

[ContainerInfo("Profiles", "/id")]
public class ProfileDTO : IItem
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonProperty("type")]
    public string Type { get; set; } = nameof(ProfileDTO);

    string IItem.PartitionKey => GetPartitionKeyValue();
    protected virtual string GetPartitionKeyValue() => Id;

    public string LastUsedBGGUsername { get; set; } = string.Empty;
}