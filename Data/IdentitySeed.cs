// File: Data/IdentitySeed.cs
using HospOps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HospOps.Data;

public static class IdentitySeed
{
    public static async Task EnsureSeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Roles
        await EnsureRoleAsync(roleMgr, "Admin");
        await EnsureRoleAsync(roleMgr, "User");

        // Admin user
        var adminUserName = "admin"; // adjust if you prefer an email
        var admin = await userMgr.FindByNameAsync(adminUserName);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminUserName,
                Email = "admin@hospops.local",
                EmailConfirmed = true
            };
            var create = await userMgr.CreateAsync(admin, "ChangeMe123!");
            if (!create.Succeeded)
            {
                var msg = string.Join(", ", create.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user: {msg}");
            }
        }
        if (!await userMgr.IsInRoleAsync(admin, "Admin"))
            await userMgr.AddToRoleAsync(admin, "Admin");
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> rm, string role)
    {
        if (!await rm.RoleExistsAsync(role))
            await rm.CreateAsync(new IdentityRole(role));
    }
}
