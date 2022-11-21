using BoardGameTracker.Application.Authentication.DTO;
using Refit;

namespace BoardGameTracker.Application.Authentication.Services;

public interface IAuthenticationClientInternal
{
    [Post("/Login")]
    Task<IApiResponse<AuthenticationResponse>> Login(LoginRequest request);

    [Post("/Registration")]
    Task<IApiResponse<AuthenticationResponse>> RegisterUser(RegisterRequest request);

    [Post("/RefreshToken")]
    Task<IApiResponse<AuthenticationResponse>> RefreshToken(RefreshTokenRequest request);
}