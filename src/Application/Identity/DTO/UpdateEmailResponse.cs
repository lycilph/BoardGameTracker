namespace BoardGameTracker.Application.Identity.DTO;

public class UpdateEmailResponse
{
    public bool IsSuccessful { get; set; } = false;
    public bool IsNoOp { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

    public static UpdateEmailResponse Success() => new() { IsSuccessful = true };

    public static UpdateEmailResponse NoOp() => new() { IsNoOp = true };

    public static UpdateEmailResponse Failure(string error) => new() { Error = error };
    public static UpdateEmailResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };
}