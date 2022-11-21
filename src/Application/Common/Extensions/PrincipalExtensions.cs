using BoardGameTracker.Application.Identity.Data;
using System.Security.Claims;

namespace BoardGameTracker.Application.Common.Extensions;

public static class PrincipalExtensions
{
    public static bool IsPersistent(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.IsPersistent);
        return claim != null && Convert.ToBoolean(claim.Value);
    }

    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static string? GetBGGUsername(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(CustomClaims.BGGUsername);
    }

    public static string? GetUsername(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Name);
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email);
    }
}