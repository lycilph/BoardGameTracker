using BoardGameTracker.Application.Identity.DTO;

namespace BoardGameTracker.Application.Identity.Services;

public interface IIdentityClient
{
    Task DeleteAccount(string userid);
    Task UpdateBGGUsername(string userid, string bgg_username);
    Task<UpdateAccountResponse> UpdateAccount(UpdateAccountRequest request);
    Task<UpdateEmailResponse> UpdateEmail(UpdateEmailRequest request);
    Task<UpdatePasswordResponse> UpdatePassword(UpdatePasswordRequest request);
}
