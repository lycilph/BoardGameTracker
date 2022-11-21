using BoardGameTracker.Application.Contracts;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace BoardGameTracker.Application.Authentication.Data;

[ContainerInfo("Identity", "/id")]
public class ApplicationRole : IdentityRole, IItem
{
    [JsonProperty("id")]
    public override string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    string IItem.PartitionKey => GetPartitionKeyValue();

    public ApplicationRole() => Type = nameof(ApplicationRole);

    protected virtual string GetPartitionKeyValue() => Id;

    public bool IsDeletable { get; set; } = true;
}
