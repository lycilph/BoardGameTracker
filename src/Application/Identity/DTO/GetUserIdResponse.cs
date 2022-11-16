namespace BoardGameTracker.Application.Identity.DTO;

public class GetUserIdResponse
{
    public bool IsSuccessful { get; set; } = false;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    public string UserId { get; set; } = string.Empty;

    public static GetUserIdResponse Success(string userid) => new() { IsSuccessful = true, UserId = userid };
    public static GetUserIdResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };
}
