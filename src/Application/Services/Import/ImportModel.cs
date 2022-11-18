using BoardGameTracker.Domain.Data;

namespace BoardGameTracker.Application.Services.Import;

public class ImportModel
{
    public List<BoardGame> owned_games = new();
    public List<BoardGame> games_to_add = new();
    public List<BoardGame> games_to_remove = new();
    public List<BoardGame> games_in_collection = new();
}
