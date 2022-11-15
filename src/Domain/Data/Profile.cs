using BoardGameTracker.Data;

namespace BoardGameTracker.Domain.Data;

public class Profile
{
    public List<BoardGame> Games { get; set; } = new();
    public List<Play> Plays { get; set; } = new();
}
