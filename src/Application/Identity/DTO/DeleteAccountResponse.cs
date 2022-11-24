namespace BoardGameTracker.Application.Identity.DTO;

public class DeleteAccountResponse
{
    public bool IsSuccessful { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

    public static DeleteAccountResponse Success() => new() { IsSuccessful = true };
    public static DeleteAccountResponse Failure(string error) => new() { Error = error };
    public static DeleteAccountResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };
}
