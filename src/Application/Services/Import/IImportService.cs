using BoardGameTracker.Domain.Data;

namespace BoardGameTracker.Application.Services.Import;

public interface IImportService
{
    Task<ImportModel> SyncWithBGGAsync(string username, string userid);
    Task ImportGamesAsync(string userid, List<BoardGame> owned, IEnumerable<BoardGame> to_add, IEnumerable<BoardGame> to_remove);
}