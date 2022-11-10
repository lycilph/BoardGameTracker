using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameTracker.Server.Controllers;

[Authorize(Roles = "Admin")]
public class DummyController : ApiControllerBase
{
    [HttpGet("Time")]
    public string Get()
    {
        return DateTime.Now.ToString();
    }
}
