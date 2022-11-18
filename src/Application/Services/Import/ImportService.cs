using BoardGameTracker.Application.BoardGameGeek;
using BoardGameTracker.Application.Game.Services;
using BoardGameTracker.Domain.Data;

namespace BoardGameTracker.Application.Services.Import;

public class ImportService : IImportService
{
    private readonly BoardGameGeekClient bgg_client;
    private readonly GameClient game_client;

    public ImportService(BoardGameGeekClient bgg_client, GameClient game_client)
    {
        this.bgg_client = bgg_client;
        this.game_client = game_client;
    }

    public async Task<ImportModel> SyncWithBGGAsync(string username, string userid)
    {
        var model = new ImportModel();

        // Load games from BGG
        var bgg_collection = await bgg_client.GetCollection(username);
        var bgg_owned_games = bgg_collection.Where(g => g.IsOwned()).ToList();

        // Load profile from DB
        model.owned_games = await game_client.GetCollectionAsync(userid);

        // Compare BGG and DB
        var comparer = new BoardGameComparer();
        model.games_to_add = bgg_owned_games.Except(model.owned_games, comparer).ToList();
        model.games_to_remove = model.owned_games.Except(bgg_owned_games, comparer).ToList();
        model.games_in_collection = model.owned_games.Intersect(bgg_owned_games, comparer).ToList();

        return model;
    }

    public async Task ImportGamesAsync(string userid, List<BoardGame> owned, IEnumerable<BoardGame> to_add, IEnumerable<BoardGame> to_remove)
    {
        var comparer = new BoardGameComparer();
        if (to_add.Any())
        {
            var ids = to_add.Select(g => g.Id).ToList();
            var games = await bgg_client.GetGameDetails(ids);
            owned.AddRange(games);
        }
        if (to_remove.Any())
            owned.RemoveAll(g => to_remove.Contains(g, comparer));

        await game_client.UpdateCollectionAsync(userid, owned);
    }
}
