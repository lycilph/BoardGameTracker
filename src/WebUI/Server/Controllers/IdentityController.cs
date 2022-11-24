using BoardGameTracker.Application.Authentication.Data;
using BoardGameTracker.Application.Identity.Commands;
using BoardGameTracker.Application.Identity.DTO;
using BoardGameTracker.Application.Identity.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Server.Controllers;

[Authorize(Roles = RoleConstants.UserRole)]
public class IdentityController : ApiControllerBase
{
    [HttpPost("UpdateAccount")]
    public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountRequest request)
    {
        var response = await Mediator.Send(new UpdateAccountCommand(request));

        if (response.IsNoOp)
            return NoContent();
        else if (response.IsSuccessful)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPost("UpdateEmail")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        var origin = Request.Headers["origin"];
        var response = await Mediator.Send(new UpdateEmailCommand(request, origin!));

        if (response.IsNoOp)
            return NoContent();
        else if (response.IsSuccessful)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPost("UpdatePassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        var response = await Mediator.Send(new UpdatePasswordCommand(request));

        if (response.IsSuccessful)
            return Ok(response);
        else
            return BadRequest(response);
    }

    [HttpPatch("UpdateBGGUsername")]
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

    [HttpDelete("DeleteAccount/{userid}")]
    public async Task<IActionResult> DeleteAccount([FromRoute] string userid)
    {
        var response = await Mediator.Send(new DeleteAccountCommand(userid));

        if (response.IsSuccessful)
            return Ok();
        else
            return BadRequest(response.Errors.Any() ? string.Join(",", response.Errors) : response.Error);
    }

    [HttpGet("GetUserId")]
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