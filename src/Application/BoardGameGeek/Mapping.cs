using BoardGameTracker.Domain.Data;

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

    public static BoardGame Map(ThingBoardgameDTO dto)
    {
        return new BoardGame
        {
            Id = dto.id,
            Name = dto.names.Single(n => n.type == "primary").value,
            YearPublished = dto.yearpublished.value,
            Thumbnail = dto.thumbnail,
            Image = dto.image,
            Description = dto.description,
            Rating = dto.statistics.ratings.average.value,
            Weight = dto.statistics.ratings.averageweight.value,
            MinPlayers = Convert.ToInt32(dto.minplayers.value),
            MaxPlayers = Convert.ToInt32(dto.maxplayers.value),
            Playingtime = Convert.ToInt32(dto.playingtime.value),
            MinPlaytime = Convert.ToInt32(dto.minplaytime.value),
            MaxPlaytime = Convert.ToInt32(dto.maxplaytime.value),
            MinAge = Convert.ToInt32(dto.minage.value)
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

    public static List<BoardGame> Map(BoardgamesListDTO<ThingBoardgameDTO> dtos)
    {
        return dtos.games.Select(Map).ToList();
    }
}
