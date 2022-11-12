using BoardGameTracker.Application.Common.Extensions;
using BoardGameTracker.Data;

namespace BoardGameTracker.Application.BoardGameGeek;

public class BoardGameGeekClient
{
    private readonly HttpClient client;

    public BoardGameGeekClient(HttpClient client)
    {
        this.client = client;
    }

    public async Task<IEnumerable<BoardGame>> GetHotnessAsync()
    {
        var dtos = await client.GetFromXmlAsync<BoardgamesListDTO<HotnessBoardgameDTO>>("hot?boardgame") ??
            new BoardgamesListDTO<HotnessBoardgameDTO>();
        return Mapping.Map(dtos);
    }
}
