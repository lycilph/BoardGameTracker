namespace BoardGameTracker.Application.Game.DTO;

public class GameResponse
{
    public bool IsSuccessful { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<BoardGameDTO> Games { get; set; } = new List<BoardGameDTO>();

    public static GameResponse Success() => new() { IsSuccessful = true };
    public static GameResponse Success(IEnumerable<BoardGameDTO> games) => new() { IsSuccessful = true, Games = games };

    public static GameResponse Failure(string error) => new() { Error = error };
    public static GameResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };
}