using BoardGameTracker.Application.Authentication.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BoardGameTracker.Application.Identity.Services;

public interface ITokenService
{
    SigningCredentials GetSigningCredentials();
    Task<List<Claim>> GetClaimsAsync(ApplicationUser user, bool is_persistent);
    JwtSecurityToken GenerateTokenOptions(SigningCredentials credentials, List<Claim> claims);
    string GenerateToken(JwtSecurityToken options);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    Task<Tuple<string, string>> GenerateTokensAsync(ApplicationUser user, bool is_persistent);
}