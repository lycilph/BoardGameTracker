using BoardGameTracker.Application.Authentication.Data;
using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Infrastructure.Storage.Contracts;

namespace BoardGameTracker.Infrastructure.Storage.Services;

public class ApplicationContext : IApplicationContext
{
    public IRepository<ApplicationUser> Users { get; private set; }
    public IRepository<ApplicationRole> Roles { get; private set; }

    public ApplicationContext(IRepository<ApplicationUser> users_repository, IRepository<ApplicationRole> roles_repository)
    {
        Users = users_repository;
        Roles = roles_repository;
    }
}
