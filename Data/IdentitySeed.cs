// File: Data/IdentitySeed.cs
using HospOps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Data;

public static class IdentitySeed
{
    /// <summary>
    /// Ensure DB is migrated, Admin role exists, and a default admin user is present.
    /// Reads optional config keys: Seed:AdminEmail, Seed:AdminPassword, Seed:AdminUserName.
    /// </summary>
    public static async Task EnsureSeedAsync(IServiceProvider services)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var env = services.GetRequiredService<IHostEnvironment>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeed");

        var context = services.GetRequiredService<HospOpsContext>();
        await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to create Admin role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
            logger.LogInformation("Created role {Role}", adminRole);
        }

        var adminEmail = configuration["Seed:AdminEmail"] ?? "admin@hospops.local";
        var adminUserName = configuration["Seed:AdminUserName"] ?? adminEmail;
        var adminPassword = configuration["Seed:AdminPassword"] ?? "ChangeMe123!"; // change in production

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to create admin user: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
            logger.LogInformation("Created default admin user {Email}", adminEmail);
        }

        if (!await userManager.IsInRoleAsync(admin, adminRole))
        {
            var addRoleResult = await userManager.AddToRoleAsync(admin, adminRole);
            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to add admin to role: " + string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }
            logger.LogInformation("Added {Email} to role {Role}", adminEmail, adminRole);
        }
    }
}