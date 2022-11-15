namespace BoardGameTracker.Application.Identity.DTO;

public class AuthenticationResponse
{
    public bool IsSuccessful { get; set; } = false;
    public bool IsNoOp { get; set; } = false;
    public string Error { get; set; } = string.Empty;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;

    public static AuthenticationResponse Success() => 
        new() { IsSuccessful = true };
    public static AuthenticationResponse Success(string token, string refresh_token) => 
        new() { IsSuccessful = true, Token = token, RefreshToken = refresh_token };

    public static AuthenticationResponse Failure(string error) => new() { Error = error };
    public static AuthenticationResponse Failure(IEnumerable<string> errors) => new() { Errors = errors };

    public static AuthenticationResponse NoOp() => new() { IsNoOp = true };
}
