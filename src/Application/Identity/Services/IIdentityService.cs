using BoardGameTracker.Application.Authentication.Data;
using Microsoft.AspNetCore.Identity;

namespace BoardGameTracker.Application.Identity.Services;

public interface IIdentityService
{
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<IdentityResult> CreateRoleAsync(ApplicationRole role);

    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

    Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string role);

    Task<ApplicationUser?> FindUserByIdAsync(string id);
    Task<ApplicationUser?> FindUserByEmailAsync(string email);
    Task<ApplicationUser?> FindUserByNameAsync(string username);

    Task<ApplicationRole?> FindRoleByNameAsync(string name);

    Task<IEnumerable<string>> GetRolesForUserAsync(ApplicationUser user);

    string NormalizeUsername(string name);
    string NormalizeEmail(string email);

    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    string HashPassword(ApplicationUser user, string password);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string current_password, string new_password);

    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string code);
    Task<IdentityResult> SetEmailAsync(ApplicationUser user, string email);
    Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
}
