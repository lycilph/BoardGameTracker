namespace BoardGameTracker.Application.Identity.DTO;

public class UpdatePasswordResponse
{
    public bool IsSuccessful { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

    public static UpdatePasswordResponse Success() => new() { IsSuccessful = true };

    public static UpdatePasswordResponse Failure(string error) => new() { Error = error };
    public static UpdatePasswordResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };
}
