using System.Diagnostics.CodeAnalysis;

namespace BoardGameTracker.Domain.Data;

public class BoardGameComparer : EqualityComparer<BoardGame>
{
    public override bool Equals(BoardGame? x, BoardGame? y)
    {
        if (x is null || y is null)
            return false;

        if (ReferenceEquals(x, y))
            return true;

        return x.Id == y.Id;
    }

    public override int GetHashCode([DisallowNull] BoardGame obj) => obj.Id.GetHashCode();
}
