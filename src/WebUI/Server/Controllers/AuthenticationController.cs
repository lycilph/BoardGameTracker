using BoardGameTracker.Application.Authentication.Commands;
using BoardGameTracker.Application.Authentication.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Server.Controllers;

public class AuthenticationController : ApiControllerBase
{
    [HttpPost("Registration")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        var origin = Request.Headers["origin"];
        var response = await Mediator.Send(new RegisterCommand(request, origin!));

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

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userid, [FromQuery] string code)
    {
        var response = await Mediator.Send(new ConfirmEmailCommand(userid, code));

        if (response.IsSuccessful)
            return Redirect("/authentication/login");
        else
            return BadRequest(response.Errors.Any() ? response.Errors : response.Error);
    }
}
