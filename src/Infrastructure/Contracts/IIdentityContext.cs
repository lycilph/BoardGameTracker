using BoardGameTracker.Application.Contracts;
using BoardGameTracker.Application.Identity.Data;

namespace BoardGameTracker.Infrastructure.Contracts;

public interface IIdentityContext
{
    IRepository<ApplicationUser> Users { get; }
    IRepository<ApplicationRole> Roles { get; }
}