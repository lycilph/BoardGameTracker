using BoardGameTracker.Application.Identity.DTO;

namespace BoardGameTracker.Application.Identity.Services;

public interface IIdentityClient
{
    Task UpdateBGGUsername(string userid, string bgg_username);
    Task<UpdateAccountResponse> UpdateAccount(UpdateAccountRequest request);
    Task<UpdateEmailResponse> UpdateEmail(UpdateEmailRequest request);
}
