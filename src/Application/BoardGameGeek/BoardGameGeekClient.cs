using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Domain.Data;

namespace BoardGameTracker.Application.BoardGameGeek;

public class BoardGameGeekClient
{
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
}
