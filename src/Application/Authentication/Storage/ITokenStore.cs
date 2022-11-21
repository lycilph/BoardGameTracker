namespace BoardGameTracker.Application.Authentication.Storage;

public interface ITokenStore
{
    Task<string> GetTokenAsync();
    Task<Tuple<string, string, bool>> GetTokensAsync();
    Task SetTokensAsync(string token, string refresh_token, bool persistent);
    Task RemoveTokensAsync();
}