using BoardGameTracker.Application.Contracts;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace BoardGameTracker.Application.Identity.Data;

[ContainerInfo("Identity", "/id")]
public class ApplicationUser : IdentityUser, IItem
{
    [JsonProperty("id")]
    public override string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    string IItem.PartitionKey => GetPartitionKeyValue();

    public ApplicationUser() => Type = nameof(ApplicationUser);

    protected virtual string GetPartitionKeyValue() => Id;

    public List<string> RoleIds { get; set; } = new List<string>();
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow;
    public string BGGUsername { get; set; } = string.Empty;
}
