using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BoardGameTracker.Infrastructure.Identity.Storage;

public class ApplicationRoleStore :
    IQueryableRoleStore<ApplicationRole>,
    IRoleStore<ApplicationRole>
//IRoleClaimStore<ApplicationRole>
{
    private readonly IIdentityContext context;
    private readonly ILogger<ApplicationRoleStore> logger;

    public ApplicationRoleStore(IIdentityContext context, ILogger<ApplicationRoleStore> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    #region NOT IMPLEMENTED YET
    public IQueryable<ApplicationRole> Roles => throw new NotImplementedException();

    public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ApplicationRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    #endregion NOT IMPLEMENTED YET

    #region CRUD
    public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role == null)
            throw new ArgumentNullException(nameof(role));

        try
        {
            await context.Roles.CreateAsync(role, cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to create role ({msg})", ex.Message);
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }
    #endregion CRUD

    #region FindBy methods
    public async Task<ApplicationRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (normalizedRoleName == null)
            throw new ArgumentNullException(nameof(normalizedRoleName));

        var roles = await context.Roles.GetAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
        return roles.FirstOrDefault()!;
    }
    #endregion FindBy methods

    #region Get properties
    public Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(GetUserProperty(role, role => role.Name, cancellationToken));
    }

    private static T GetUserProperty<T>(ApplicationRole role, Func<ApplicationRole, T> accessor, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role == null)
            throw new ArgumentNullException(nameof(role));

        return accessor(role);
    }
    #endregion Get properties

    #region Set properties
    public Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        SetUserProperty(role, normalizedName, (r, m) => r.NormalizedName = normalizedName, cancellationToken);
        return Task.CompletedTask;
    }

    private static void SetUserProperty<T>(ApplicationRole role, T value, Action<ApplicationRole, T> setter, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role == null) throw new ArgumentNullException(nameof(role));
        if (value == null) throw new ArgumentNullException(nameof(value));

        setter(role, value);
    }
    #endregion Set properties

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
