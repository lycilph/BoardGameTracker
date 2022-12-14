using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Domain.Data;

namespace BoardGameTracker.Application.BoardGameGeek;

public class BoardGameGeekClient
{
    private const int ChunkSize = 50;
    private const int Delay = 1000;

    private readonly HttpClient client;

    public BoardGameGeekClient(HttpClient client)
    {
        this.client = client;
    }

    public async Task<bool> UserExists(string username)
    {
        var dto = await client.GetFromXmlAsync<BoardGameGeekUserDTO>($"user?name={username}") ?? new BoardGameGeekUserDTO();
        return !string.IsNullOrWhiteSpace(dto.id);
    }

    public async Task<List<BoardGame>> GetHotnessAsync()
    {
        var dtos = await client.GetFromXmlAsync<BoardgamesListDTO<HotnessBoardgameDTO>>("hot?boardgame") ??
            new BoardgamesListDTO<HotnessBoardgameDTO>();
        return Mapping.Map(dtos);
    }

    public async Task<List<BoardGame>> GetCollection(string username)
    {
        var query = $"collection?username={username}";
        var dtos = await client.GetFromXmlAsync<BoardgamesListDTO<CollectionBoardgameDTO>>(query) ??
            new BoardgamesListDTO<CollectionBoardgameDTO>();
        return Mapping.Map(dtos);
    }

    public async Task<BoardGame> GetGameDetails(string id)
    {
        var query = $"thing?id={id}&stats=1";
        var dtos = await client.GetFromXmlAsync<BoardgamesListDTO<ThingBoardgameDTO>>(query) ??
            new BoardgamesListDTO<ThingBoardgameDTO>();
        return Mapping.Map(dtos.games.First());
    }

    public async Task<List<BoardGame>> GetGameDetails(List<string> ids)
    {
        // Do this in chunks of 50
        List<BoardGame> result = new();
        foreach (var chunk in ids.Chunk(ChunkSize))
        {
            var id_str = string.Join(",", chunk);
            var query = $"thing?id={id_str}&stats=1";
            var dtos = await client.GetFromXmlAsync<BoardgamesListDTO<ThingBoardgameDTO>>(query) ??
                new BoardgamesListDTO<ThingBoardgameDTO>();
            result.AddRange(Mapping.Map(dtos));
            // Be nice towards BGG
            await Task.Delay(Delay);
        }
        return result;
    }
}
