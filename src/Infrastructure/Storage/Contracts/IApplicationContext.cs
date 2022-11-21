using BoardGameTracker.Application.Authentication.Data;
using BoardGameTracker.Application.Contracts;

namespace BoardGameTracker.Infrastructure.Storage.Contracts;

public interface IApplicationContext
{
    IRepository<ApplicationUser> Users { get; }
    IRepository<ApplicationRole> Roles { get; }
}