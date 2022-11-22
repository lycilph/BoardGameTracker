using BoardGameTracker.Application.Identity.DTO;

namespace BoardGameTracker.Application.Identity.Services;

public interface IIdentityClient
{
    Task<UpdateAccountResponse> UpdateAccount(UpdateAccountRequest request);
    Task UpdateBGGUsername(string userid, string bgg_username);
}
