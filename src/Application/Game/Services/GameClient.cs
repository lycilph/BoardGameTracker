using BoardGameTracker.Domain.Data;
using System.Net.Http.Json;

namespace BoardGameTracker.Application.Game.Services;

public class GameClient
{
    private readonly HttpClient client;

    public GameClient(HttpClient client)
    {
        this.client = client;
    }

    public async Task<Profile> GetProfile(string id)
    {
        return await client.GetFromJsonAsync<Profile>($"Profile/{id}") ?? new Profile();
    }

    public async Task UpdateProfile(Profile profile)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"profile");
        request.Content = JsonContent.Create(profile);
        await client.SendAsync(request);
    }
}
