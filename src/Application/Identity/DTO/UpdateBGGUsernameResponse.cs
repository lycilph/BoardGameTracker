namespace BoardGameTracker.Application.Identity.DTO;

public class UpdateBGGUsernameResponse
{
    public bool IsSuccessful { get; set; } = false;
    public bool IsNoOp { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

    public static UpdateBGGUsernameResponse Success() => new() { IsSuccessful = true };

    public static UpdateBGGUsernameResponse NoOp() => new() { IsNoOp = true };

    public static UpdateBGGUsernameResponse Failure(string error) => new() { Error = error };
    public static UpdateBGGUsernameResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };

}
