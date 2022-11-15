using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BoardGameTracker.Infrastructure.Identity.Services;

public class TokenService : ITokenService
{
    private readonly JWTSettings settings;
    private readonly IIdentityService identity_service;

    public TokenService(IOptions<JWTSettings> options, IIdentityService identity_service)
    {
        settings = options.Value;
        this.identity_service = identity_service;
    }

    public SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(settings.SecurityKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public async Task<List<Claim>> GetClaimsAsync(ApplicationUser user, bool is_persistent)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.IsPersistent, is_persistent.ToString())
            };

        if (!string.IsNullOrWhiteSpace(user.BGGUsername))
            claims.Add(new Claim(CustomClaims.BGGUsername, user.BGGUsername));

        var roles = await identity_service.GetRolesForUserAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    public JwtSecurityToken GenerateTokenOptions(SigningCredentials credentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken(
            issuer: settings.ValidIssuer,
            audience: settings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(settings.ExpiryInMinutes)),
            signingCredentials: credentials);

        return tokenOptions;
    }

    public string GenerateToken(JwtSecurityToken options)
    {
        return new JwtSecurityTokenHandler().WriteToken(options);
    }

#pragma warning disable IDE0063
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
#pragma warning restore IDE0063

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecurityKey)),
            ValidIssuer = settings.ValidIssuer,
            ValidAudience = settings.ValidAudience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public async Task<Tuple<string, string>> GenerateTokensAsync(ApplicationUser user, bool is_persistent)
    {
        var credentials = GetSigningCredentials();
        var claims = await GetClaimsAsync(user, is_persistent);
        var options = GenerateTokenOptions(credentials, claims);

        var token = GenerateToken(options);
        var refresh_token = GenerateRefreshToken();

        return Tuple.Create(token, refresh_token);
    }
}
