using Blazored.LocalStorage;
using Blazored.SessionStorage;
using BoardGameTracker.Application.Authentication.Storage;

namespace BoardGameTracker.Infrastructure.Storage.Services;

public class TokenStore : ITokenStore
{
    private const string token_key = "AuthenticationToken";
    private const string refresh_token_key = "RefreshToken";

    private readonly ILocalStorageService local_storage;
    private readonly ISessionStorageService session_storage;

    public TokenStore(ILocalStorageService local_storage, ISessionStorageService session_storage)
    {
        this.local_storage = local_storage;
        this.session_storage = session_storage;
    }

    public async Task<string> GetTokenAsync()
    {
        var token = await session_storage.GetItemAsync<string>(token_key);

        if (string.IsNullOrEmpty(token))
            token = await local_storage.GetItemAsync<string>(token_key);

        return token;
    }

    public async Task<Tuple<string, string, bool>> GetTokensAsync()
    {
        var was_persistent = false;
        var token = await session_storage.GetItemAsync<string>(token_key);
        var refresh_token = await session_storage.GetItemAsync<string>(refresh_token_key);

        // If there was no tokens in the session storage, try in local storage (ie. remember me is enabled)
        if (string.IsNullOrEmpty(token))
        {
            was_persistent = true;
            token = await local_storage.GetItemAsync<string>(token_key);
            refresh_token = await local_storage.GetItemAsync<string>(refresh_token_key);
        }

        return Tuple.Create(token, refresh_token, was_persistent);
    }

    public async Task SetTokensAsync(string token, string refresh_token, bool persistent)
    {
        if (persistent)
        {
            await local_storage.SetItemAsync(token_key, token);
            await local_storage.SetItemAsync(refresh_token_key, refresh_token);
        }
        else
        {
            await session_storage.SetItemAsync(token_key, token);
            await session_storage.SetItemAsync(refresh_token_key, refresh_token);
        }
    }

    public async Task RemoveTokensAsync()
    {
        await local_storage.RemoveItemAsync(token_key);
        await local_storage.RemoveItemAsync(refresh_token_key);
        await session_storage.RemoveItemAsync(token_key);
        await session_storage.RemoveItemAsync(refresh_token_key);
    }
}
