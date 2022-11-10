using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Infrastructure.Contracts;

namespace BoardGameTracker.Infrastructure.Identity.Storage;

public class IdentityContext : IIdentityContext
{
    public IRepository<ApplicationUser> Users { get; private set; }
    public IRepository<ApplicationRole> Roles { get; private set; }

    public IdentityContext(IRepository<ApplicationUser> users_repository, IRepository<ApplicationRole> roles_repository)
    {
        Users = users_repository;
        Roles = roles_repository;
    }
}
