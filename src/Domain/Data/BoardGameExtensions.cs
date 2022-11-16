namespace BoardGameTracker.Domain.Data;

public static class BoardGameExtensions
{
    public static bool IsOwned(this BoardGame game)
    {
        return game.Status.Contains(BoardGameStates.Owned);
    }
}
