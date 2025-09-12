// File: Pages/Admin/Users/Index.cshtml.cs
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<IdentityRole> _roles;

    public IndexModel(UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles)
    {
        _users = users;
        _roles = roles;
    }

    public class UserRow
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }

    public List<UserRow> Users { get; private set; } = new();

    public async Task OnGet()
    {
        var list = await _users.Users.AsNoTracking().ToListAsync();
        Users = new List<UserRow>(list.Count);
        foreach (var u in list)
        {
            Users.Add(new UserRow
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                IsAdmin = await _users.IsInRoleAsync(u, "Admin"),
                IsActive = (u as ApplicationUser)?.IsActive ?? true
            });
        }
    }

    public async Task<IActionResult> OnPostToggleAdmin(string id, bool makeAdmin)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null) return NotFound();

        const string role = "Admin";
        if (!await _roles.RoleExistsAsync(role))
            await _roles.CreateAsync(new IdentityRole(role));

        IdentityResult result = makeAdmin
            ? await _users.AddToRoleAsync(user, role)
            : await _users.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
            TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
        else
            TempData["Success"] = makeAdmin ? "Granted Admin." : "Revoked Admin.";

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSetActive(string id, bool active)
    {
        var user = await _users.FindByIdAsync(id) as ApplicationUser;
        if (user is null) return NotFound();
        user.IsActive = active;
        var res = await _users.UpdateAsync(user);
        TempData["Success"] = res.Succeeded ? "Saved." : string.Join("; ", res.Errors.Select(e => e.Description));
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostResetPassword(string id, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            TempData["Error"] = "Password required.";
            return RedirectToPage();
        }
        var user = await _users.FindByIdAsync(id);
        if (user is null) return NotFound();
        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var res = await _users.ResetPasswordAsync(user, token, newPassword);
        TempData["Success"] = res.Succeeded ? "Password reset." : string.Join("; ", res.Errors.Select(e => e.Description));
        return RedirectToPage();
    }
}
