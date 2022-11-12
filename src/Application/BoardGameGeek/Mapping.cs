using BoardGameTracker.Data;

namespace BoardGameTracker.Application.BoardGameGeek;

public static class Mapping
{
    public static BoardGame Map(HotnessBoardgameDTO dto)
    {
        return new BoardGame
        {
            Id = dto.id,
            Name = dto.name.value,
            YearPublished = dto.yearpublished.value,
            Thumbnail= dto.thumbnail.value
        };
    }

    public static IEnumerable<BoardGame> Map(BoardgamesListDTO<HotnessBoardgameDTO> dtos)
    {
        return dtos.games.Select(Map).ToList();
    }
}
