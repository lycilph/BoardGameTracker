using BoardGameTracker.Application.Identity.DTO;
using Refit;

namespace BoardGameTracker.Application.Identity.Services;

public interface IAuthenticationClient
{
    Task<IApiResponse<AuthenticationResponse>> Login(LoginRequest request);
    Task<IApiResponse<AuthenticationResponse>> RegisterUser(RegisterRequest request);
    Task<IApiResponse<AuthenticationResponse>> RefreshToken();
    Task Logout();
}
