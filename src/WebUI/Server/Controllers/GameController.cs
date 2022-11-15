using BoardGameTracker.Application.Game.Commands;
using BoardGameTracker.Application.Game.Queries;
using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Domain.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Server.Controllers;

[Authorize(Roles = RoleConstants.UserRole)]
public class GameController : ApiControllerBase
{
    [HttpGet("Profile/{id}")]
    public async Task<IActionResult> Profile(string id)
    {
        var profile = await Mediator.Send(new ProfileQuery(id));

        return Ok(profile);
    }

    [HttpPut("Profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] Profile profile)
    {
        await Mediator.Send(new UpdateProfileCommand(profile));

        return Ok();
    }
}
