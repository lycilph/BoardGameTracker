using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Infrastructure.Identity.Storage;

public class ApplicationUserStore :
    IQueryableUserStore<ApplicationUser>,
    IUserStore<ApplicationUser>,
    IUserRoleStore<ApplicationUser>,
    IUserEmailStore<ApplicationUser>,
    IUserPasswordStore<ApplicationUser>,
    IUserPhoneNumberStore<ApplicationUser>
{
    private readonly IIdentityContext context;
    private readonly ILogger<ApplicationUserStore> logger;

    public ApplicationUserStore(IIdentityContext context, ILogger<ApplicationUserStore> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public IQueryable<ApplicationUser> Users => throw new NotImplementedException();

    #region NOT IMPLEMENTED YET
    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetPhoneNumberAsync(ApplicationUser user, string? phoneNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    #endregion NOT IMPLEMENTED YET

    #region CRUD
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        try
        {
            await context.Users.CreateAsync(user, cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to create user ({msg})", ex.Message);
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        try
        {
            await context.Users.UpdateAsync(user, cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }
    #endregion CRUD

    #region Misc methods
    public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

        var roles = await context.Roles.GetAsync(r => r.NormalizedName == roleName, cancellationToken);
        var role = roles.FirstOrDefault();
        if (role == null)
            throw new InvalidOperationException($"Role {roleName} not found.");

        user.RoleIds.Add(role.Id);
        await context.Users.UpdateAsync(user, cancellationToken);
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentNullException(nameof(roleName));

        var roles = await context.Roles.GetAsync(r => r.NormalizedName == roleName, cancellationToken);
        var role = roles.FirstOrDefault();

        if (role != null)
            return user.RoleIds.Contains(role.Id);

        return false;
    }
    #endregion Misc methods

    #region FindBy methods
    public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (normalizedUserName == null)
            throw new ArgumentNullException(nameof(normalizedUserName));

        var users = await context.Users.GetAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
        return users.FirstOrDefault()!;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (normalizedEmail == null)
            throw new ArgumentNullException(nameof(normalizedEmail));

        var users = await context.Users.GetAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        return users.FirstOrDefault()!;
    }
    #endregion FindBy methods

    #region Get properties
    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(GetUserProperty(user, user => user.UserName, cancellationToken));
    }

    public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(GetUserProperty(user, user => user.Email, cancellationToken));
    }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(GetUserProperty(user, user => user.Id, cancellationToken));
    }

    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(GetUserProperty(user, user => user.PasswordHash, cancellationToken));
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var roles = await context.Roles.GetAsync(user.RoleIds, cancellationToken);
        return roles.Select(r => r.Name ?? "Unknown").ToList();
    }

    private static T GetUserProperty<T>(ApplicationUser user, Func<ApplicationUser, T> accessor, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return accessor(user);
    }
    #endregion Get properties

    #region Set properties
    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        SetUserProperty(user, passwordHash, (u, m) => u.PasswordHash = passwordHash, cancellationToken);
        return Task.CompletedTask;
    }

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        SetUserProperty(user, normalizedName, (u, m) => u.NormalizedUserName = normalizedName, cancellationToken);
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        SetUserProperty(user, normalizedEmail, (u, m) => u.NormalizedEmail = normalizedEmail, cancellationToken);
        return Task.CompletedTask;
    }

    private static void SetUserProperty<T>(ApplicationUser user, T value, Action<ApplicationUser, T> setter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null) throw new ArgumentNullException(nameof(user));
        if (value == null) throw new ArgumentNullException(nameof(value));

        setter(user, value);
    }
    #endregion Set properties

    public void Dispose() 
    {
        GC.SuppressFinalize(this);
    }
}