using System.Security.Claims;

namespace BoardGameTracker.Application.Common.Extensions;

public static class PrincipalExtensions
{
    public static bool IsPersistent(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.IsPersistent);
        return claim != null && Convert.ToBoolean(claim.Value);
    }
}
