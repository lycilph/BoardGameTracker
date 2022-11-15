﻿using BoardGameTracker.Application.Identity.Commands;
using BoardGameTracker.Application.Identity.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Server.Controllers;

public class AuthenticationController : ApiControllerBase
{
    [HttpPost("Registration")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        var response = await Mediator.Send(new RegisterCommand(request));

        if (response.IsSuccessful)
            return CreatedAtAction(nameof(RegisterUser), request);
        else
            return BadRequest(response.Errors.Any() ? response.Errors : response.Error);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await Mediator.Send(new LoginCommand(request));

        if (response.IsSuccessful)
            return Ok(response);
        else
            return Unauthorized(response.Errors.Any() ? response.Errors : response.Error);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await Mediator.Send(new RefreshTokenCommand(request));

        if (response.IsSuccessful)
            return Ok(response);
        else
            return BadRequest(response.Errors.Any() ? response.Errors : response.Error);
    }

    [HttpPatch("UpdateBGGUsername")]
    public async Task<IActionResult> UpdateBGGUsername([FromBody] UpdateBGGUsernameRequest request)
    {
        var response = await Mediator.Send(new UpdateBGGUsernameCommand(request));

        if (response.IsSuccessful)
            return Ok(response);
        else
            return BadRequest(response.Errors.Any() ? response.Errors : response.Error);
    }
}
