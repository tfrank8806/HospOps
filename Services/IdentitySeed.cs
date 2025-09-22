// ============================================================================
// File: Services/IdentitySeed.cs   (REPLACE ENTIRE FILE)
// ============================================================================
using System.Security.Claims;
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospOps.Services
{
    public static class IdentitySeed
    {
        public static async Task EnsureSeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeed");
            var cfg = sp.GetRequiredService<IConfiguration>();
            var context = sp.GetRequiredService<HospOpsContext>();
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

            // Make sure DB is up-to-date
            await context.Database.MigrateAsync();

            // --- Roles ---
            string[] roles = ["Admin", "Manager", "User"];
            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(r));
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError("Failed creating role {Role}: {Errors}", r,
                            string.Join("; ", roleResult.Errors.Select(e => $"{e.Code}:{e.Description}")));
                    }
                }
            }

            // --- Admin user (from config or defaults) ---
            var adminEmail = cfg["Seed:AdminEmail"] ?? "admin@hospops.local";
            var adminPassword = cfg["Seed:AdminPassword"] ?? "ChangeMe123!";
            var adminDisplayName = cfg["Seed:AdminDisplayName"] ?? "System Admin";

            var admin = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = adminDisplayName,
                    LockoutEnabled = false
                };
                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    logger.LogError("Failed creating admin user {Email}: {Errors}", adminEmail,
                        string.Join("; ", createResult.Errors.Select(e => $"{e.Code}:{e.Description}")));
                    return;
                }
                logger.LogInformation("Admin user created: {Email}", adminEmail);
            }
            else
            {
                // Ensure email confirmed and not locked out
                if (!admin.EmailConfirmed)
                {
                    admin.EmailConfirmed = true;
                }
                admin.DisplayName ??= adminDisplayName;
                admin.LockoutEnabled = false;
                admin.LockoutEnd = null;

                var updateResult = await userManager.UpdateAsync(admin);
                if (!updateResult.Succeeded)
                {
                    logger.LogError("Failed updating admin user {Email}: {Errors}", adminEmail,
                        string.Join("; ", updateResult.Errors.Select(e => $"{e.Code}:{e.Description}")));
                }

                // Ensure a password exists (if user was created without one)
                var hasPwd = await userManager.HasPasswordAsync(admin);
                if (!hasPwd)
                {
                    var pwdResult = await userManager.AddPasswordAsync(admin, adminPassword);
                    if (!pwdResult.Succeeded)
                    {
                        logger.LogError("Failed setting password for admin {Email}: {Errors}", adminEmail,
                            string.Join("; ", pwdResult.Errors.Select(e => $"{e.Code}:{e.Description}")));
                    }
                }
            }

            // --- Assign roles to admin ---
            foreach (var r in new[] { "Admin", "Manager", "User" })
            {
                if (!await userManager.IsInRoleAsync(admin, r))
                {
                    var addRole = await userManager.AddToRoleAsync(admin, r);
                    if (!addRole.Succeeded)
                    {
                        logger.LogError("Failed adding role {Role} to admin {Email}: {Errors}", r, adminEmail,
                            string.Join("; ", addRole.Errors.Select(e => $"{e.Code}:{e.Description}")));
                    }
                }
            }

            // --- Optional: admin claims (handy for policies) ---
            var claims = await userManager.GetClaimsAsync(admin);
            if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                await userManager.AddClaimAsync(admin, new Claim(ClaimTypes.Role, "Admin"));
            }

            logger.LogInformation("Identity seed complete. Admin: {Email}", adminEmail);
        }
    }
}
