using BoardGameTracker.Application.Game.DTO;
using BoardGameTracker.Domain.Data;

namespace BoardGameTracker.Application.Game.Services;

public static class Mapping
{
    public static Profile Map(ProfileDTO dto)
    {
        return new Profile
        {
            Id = dto.Id,
            Games = dto.BoardGameIds.Count
        };
    }

    public static BoardGameDTO Map(BoardGame game)
    {
        return new BoardGameDTO
        {
            Id = game.Id,
            Name = game.Name,
            YearPublished = game.YearPublished,
            Image = game.Image,
            Thumbnail = game.Thumbnail
        };
    }

    public static BoardGame Map(BoardGameDTO dto)
    {
        return new BoardGame
        {
            Id = dto.Id,
            Name = dto.Name,
            YearPublished = dto.YearPublished,
            Image = dto.Image,
            Thumbnail = dto.Thumbnail
        };
    }

    public static List<BoardGameDTO> Map(List<BoardGame> games)
    {
        return games.Select(Map).ToList();
    }

    public static List<BoardGame> Map(List<BoardGameDTO> dtos)
    {
        return dtos.Select(Map).ToList();
    }
}