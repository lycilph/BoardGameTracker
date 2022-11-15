﻿using BoardGameTracker.Application.Identity.DTO;
using Refit;

namespace BoardGameTracker.Application.Identity.Services;

public interface IAuthenticationClientInternal
{
    [Post("/Login")]
    Task<IApiResponse<AuthenticationResponse>> Login(LoginRequest request);

    [Post("/Registration")]
    Task<IApiResponse<AuthenticationResponse>> RegisterUser(RegisterRequest request);

    [Post("/RefreshToken")]
    Task<IApiResponse<AuthenticationResponse>> RefreshToken(RefreshTokenRequest request);

    [Patch("/UpdateBGGUsername")]
    Task<IApiResponse<AuthenticationResponse>> UpdateBGGUsername(UpdateBGGUsernameRequest request);
}