using BoardGameTracker.Domain.Data;
using System.Globalization;

namespace BoardGameTracker.Client.Pages.Content.Models;

public class BoardGameModel
{
    public BoardGame Game { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public string YearPublished { get; set; } = string.Empty;
    public string Players { get; set; } = string.Empty;
    public string Playtime { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;

    public static BoardGameModel Create(BoardGame game)
    {
        double rating = Convert.ToDouble(game.Rating, CultureInfo.InvariantCulture);
        double weight = Convert.ToDouble(game.Weight, CultureInfo.InvariantCulture);

        return new BoardGameModel
        {
            Game = game,
            Name = game.Name,
            Thumbnail = game.Thumbnail,
            YearPublished = game.YearPublished,
            Players = $"{game.MinPlayers}-{game.MaxPlayers}",
            Playtime = $"{game.MinPlaytime}-{game.MaxPlaytime} min",
            Rating = rating.ToString("0.#"),
            Weight = weight.ToString("0.#")
        };
    }
}
