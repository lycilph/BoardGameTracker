namespace BoardGameTracker.Application.Identity.Services;

public interface IIdentityClient
{
    Task UpdateBGGUsername(string userid, string bgg_username);
}
