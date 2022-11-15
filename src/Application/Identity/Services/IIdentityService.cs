using BoardGameTracker.Application.Identity.Data;
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

    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    string HashPassword(ApplicationUser admin, string password);
}
