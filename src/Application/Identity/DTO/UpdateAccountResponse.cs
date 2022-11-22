namespace BoardGameTracker.Application.Identity.DTO;

public class UpdateAccountResponse
{
    public bool IsSuccessful { get; set; } = false;
    public bool IsNoOp { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

    public static UpdateAccountResponse Success() => new() { IsSuccessful = true };

    public static UpdateAccountResponse NoOp() => new() { IsNoOp = true };

    public static UpdateAccountResponse Failure(string error) => new() { Error = error };
    public static UpdateAccountResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };
}
