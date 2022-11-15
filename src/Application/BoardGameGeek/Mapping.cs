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

    public static BoardGame Map(CollectionBoardgameDTO dto)
    {
        return new BoardGame
        {
            Id = dto.objectid,
            Name = dto.name,
            YearPublished = dto.yearpublished,
            Thumbnail = dto.thumbnail,
            Image= dto.image,
            NumberOfPlays = dto.numplays,
            Status = dto.GetStatusList()
        };
    }

    public static List<BoardGame> Map(BoardgamesListDTO<HotnessBoardgameDTO> dtos)
    {
        return dtos.games.Select(Map).ToList();
    }
    
    public static List<BoardGame> Map(BoardgamesListDTO<CollectionBoardgameDTO> dtos)
    {
        return dtos.games.Select(Map).ToList();
    }
}
