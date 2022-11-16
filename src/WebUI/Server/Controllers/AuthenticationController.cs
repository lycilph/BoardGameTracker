using BoardGameTracker.Application.Identity.Commands;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Query;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPatch("UpdateBGGUsername"), Authorize]
    public async Task<IActionResult> UpdateBGGUsername([FromBody] UpdateBGGUsernameRequest request)
    {
        var response = await Mediator.Send(new UpdateBGGUsernameCommand(request));

        if (response.IsNoOp)
            return NoContent();
        else if (response.IsSuccessful)
            return Ok(response);
        else
            return BadRequest(response.Errors.Any() ? response.Errors : response.Error);
    }

    [HttpGet("GetUserId"), Authorize]
    public async Task<IActionResult> GetUserId()
    {
        var auth_header = Request.Headers.Authorization.First()!;
        var token = auth_header.Replace("Bearer", "").Trim();

        var response = await Mediator.Send(new GetUserIdQuery(token));

        if (response.IsSuccessful)
            return Ok(response.UserId);
        else
            return BadRequest(response.Errors);
    }
}
