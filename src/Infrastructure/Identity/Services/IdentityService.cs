﻿using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Application.Identity.Services;
using Microsoft.AspNetCore.Identity;

namespace BoardGameTracker.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> user_manager;
    private readonly RoleManager<ApplicationRole> role_manager;

    public IdentityService(UserManager<ApplicationUser> user_manager, RoleManager<ApplicationRole> role_manager)
    {
        this.user_manager = user_manager;
        this.role_manager = role_manager;
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        return await user_manager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> CreateRoleAsync(ApplicationRole role)
    {
        return await role_manager.CreateAsync(role);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    {
        return await user_manager.UpdateAsync(user);
    }

    public async Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string role)
    {
        return await user_manager.AddToRoleAsync(user, role);
    }

    public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
    {
        return await user_manager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> FindUserByNameAsync(string username)
    {
        return await user_manager.FindByNameAsync(username);
    }

    public async Task<ApplicationRole?> FindRoleByNameAsync(string name)
    {
        return await role_manager.FindByNameAsync(name);
    }

    public async Task<IEnumerable<string>> GetRolesForUserAsync(ApplicationUser user)
    {
        return await user_manager.GetRolesAsync(user);
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        return await user_manager.CheckPasswordAsync(user, password);
    }

    public string HashPassword(ApplicationUser admin, string password)
    {
        return user_manager.PasswordHasher.HashPassword(admin, password);
    }
}