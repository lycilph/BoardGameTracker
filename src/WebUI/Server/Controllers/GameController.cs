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
    [HttpGet("Collection/{userid}")]
    public async Task<IActionResult> Collection(string userid)
    {
        var response = await Mediator.Send(new CollectionQuery(userid));

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



    //[HttpGet("Profile/{id}")]
    //public async Task<IActionResult> Profile(string id)
    //{
    //    var profile = await Mediator.Send(new ProfileQuery(id));

    //    return Ok(profile);
    //}

    //[HttpPut("Profile")]
    //public async Task<IActionResult> UpdateProfile([FromBody] Profile profile)
    //{
    //    await Mediator.Send(new UpdateProfileCommand(profile));

    //    return Ok();
    //}
}
