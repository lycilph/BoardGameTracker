namespace BoardGameTracker.Application.Identity.DTO;

public class GetUserInfoResponse
{
    public bool IsSuccessful { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    public DateTime AccountCreated { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;

    public static GetUserInfoResponse Success(DateTime created, DateTime active) => new() { IsSuccessful = true, AccountCreated = created, LastActive = active };
    public static GetUserInfoResponse Failure(string error) => new() { Error = error };
    public static GetUserInfoResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };
}
