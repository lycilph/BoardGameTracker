using BoardGameTracker.Application.Identity.Data;
using BoardGameTracker.Application.Identity.Services;
using BoardGameTracker.Infrastructure.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BoardGameTracker.Infrastructure;

#pragma warning disable IDE0063
public static class ConfigureData
{
    public static void SeedIdentityData(this IApplicationBuilder builder)
    {
        using (var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var provider = scope.ServiceProvider;

            var logger = provider.GetRequiredService<ILogger<ApplicationUser>>();
            var identity_service = provider.GetRequiredService<IIdentityService>();
            var options = provider.GetRequiredService<IOptions<IdentitySeedData>>().Value;

            var admin_role = new ApplicationRole
            {
                Name = RoleConstants.AdministratorRole,
                IsDeletable = false
            };
            if (identity_service.FindRoleByNameAsync(admin_role.Name).Result == null)
            {
                logger.LogInformation("Admin role not found - creating now");
                identity_service.CreateRoleAsync(admin_role).Wait();
            }

            var user_role = new ApplicationRole
            {
                Name = RoleConstants.UserRole,
                IsDeletable = false
            };
            if (identity_service.FindRoleByNameAsync(user_role.Name).Result == null)
            {
                logger.LogInformation("User role not found - creating now");
                identity_service.CreateRoleAsync(user_role).Wait();
            }

            var admin = new ApplicationUser
            {
                UserName = options.Username,
                Email = options.Email,
                EmailConfirmed = true
            };
            if (identity_service.FindUserByEmailAsync(admin.Email).Result == null)
            {
                logger.LogInformation("Admin not found - creating now");
                identity_service.CreateUserAsync(admin, options.Password).Wait();
                identity_service.AddUserToRoleAsync(admin, admin_role.Name).Wait();
                identity_service.AddUserToRoleAsync(admin, user_role.Name).Wait();
            }
        }
    }
}
#pragma warning restore IDE0063