using BoardGameTracker.Application.Game.DTO;
using BoardGameTracker.Domain.Data;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace BoardGameTracker.Application.Game.Services;

public class GameClient
{
    private readonly HttpClient client;
    private readonly ILogger<GameClient> logger;

    public GameClient(HttpClient client, ILogger<GameClient> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task<List<BoardGame>> GetCollectionAsync(string userid)
    {
        try
        {
            var dtos = await client.GetFromJsonAsync<List<BoardGameDTO>>($"Collection/{userid}") ?? new List<BoardGameDTO>();
            return Mapping.Map(dtos);
        }
        catch (HttpRequestException e) 
        {
            logger.LogError("Error: {error}", e.Message);
        }
        return new();
    }

    public async Task UpdateGamesAsync(string userid, List<BoardGame> games)
    {
        var dtos = Mapping.Map(games);
        var update_request = new UpdateCollectionRequest { UserId = userid, Games = dtos };

        var request = new HttpRequestMessage(HttpMethod.Put, "Collection")
        {
            Content = JsonContent.Create(update_request)
        };

        await client.SendAsync(request);

        await Task.CompletedTask;
    }

    //public List<Play> GetPlays(string userid) => new();
    //public void LogPlay() { }
    //public void UpdatePlay(string playid, Play play) { }

    //public async Task<Profile> GetProfile(string userid)
    //{
    //    return await client.GetFromJsonAsync<Profile>($"Profile/{userid}") ?? new Profile();
    //}

    //public async Task UpdateProfile(Profile profile)
    //{
    //    var request = new HttpRequestMessage(HttpMethod.Put, $"profile");
    //    request.Content = JsonContent.Create(profile);
    //    await client.SendAsync(request);
    //}
}
