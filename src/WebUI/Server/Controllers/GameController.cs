using BoardGameTracker.Application.Game.Commands;
using BoardGameTracker.Application.Game.DTO;
using BoardGameTracker.Application.Game.Queries;
using BoardGameTracker.Application.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Server.Controllers;

[Authorize(Roles = RoleConstants.UserRole)]
public class GameController : ApiControllerBase
{
    [HttpGet("Collection/{id}")]
    public async Task<IActionResult> GetCollection(string id)
    {
        var response = await Mediator.Send(new CollectionQuery(id));

        if (response.IsSuccessful)
            return Ok(response.Games);
        else
            return NotFound(response.Errors.Any() ? response.Errors : response.Error);
    }

    [HttpPut("Collection")]
    public async Task<IActionResult> UpdateCollection([FromBody] UpdateCollectionRequest request)
    {
        var response = await Mediator.Send(new UpdateCollectionCommand(request));

        if (response.IsSuccessful)
            return Ok();
        else
            return NotFound(response.Errors.Any() ? response.Errors : response.Error);
    }

    [HttpGet("Profile/{id}")]
    public async Task<IActionResult> GetProfile(string id)
    {
        var profile = await Mediator.Send(new ProfileQuery(id));

        return Ok(profile);
    }
}
