// File: HospOps/Pages/Admin/Users/Edit.cshtml.cs
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HospOps.Data;
using HospOps.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly HospOpsContext _db;
        public EditModel(HospOpsContext db) => _db = db;

        public class EditUserInput
        {
            [Required]
            public int Id { get; set; }

            public string Username { get; set; } = string.Empty;

            [EmailAddress]
            public string? Email { get; set; }

            [Required]
            public string Role { get; set; } = "User";

            public bool IsActive { get; set; }
        }

        [BindProperty]
        public EditUserInput Input { get; set; } = new();

        public async Task<IActionResult> OnGet(int id)
        {
            var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.ToString());
            if (u == null) return NotFound();

            Input = MapFromEntity(u);
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (!ModelState.IsValid) return Page();

            var entity = await _db.Users.FirstOrDefaultAsync(x => x.Id == Input.Id.ToString());
            if (entity == null) return NotFound();

            var currentUserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var cid) ? cid : 0;

            // Guards: last Admin & self-deactivation
            var userRoles = await _db.UserRoles
                .Where(ur => ur.UserId == entity.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();
            bool isAdmin = userRoles.Contains("Admin");

            bool demotingAdmin = isAdmin && !string.Equals(Input.Role, "Admin", StringComparison.OrdinalIgnoreCase);
            bool deactivating = entity.IsActive && !Input.IsActive;

            if ((demotingAdmin || (deactivating && isAdmin)))
            {
                var adminCount = await _db.Users.CountAsync(u => u.IsActive && 
                    _db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == "Admin"));
                if (adminCount <= 1)
                {
                    ModelState.AddModelError(string.Empty, "You cannot remove or deactivate the last Admin account.");
                    Input = MapFromEntity(entity);
                    return Page();
                }
            }

            if (int.TryParse(entity.Id, out var entityId) && entityId == currentUserId && deactivating)
            {
                ModelState.AddModelError(string.Empty, "You cannot deactivate your own account while signed in.");
                Input = MapFromEntity(entity);
                return Page();
            }

            // Apply updates
            entity.Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim();
            entity.IsActive = Input.IsActive;

            // Update the user's roles via the UserRoles table
            var userRolesToRemove = _db.UserRoles.Where(ur => ur.UserId == entity.Id);
            _db.UserRoles.RemoveRange(userRolesToRemove);
            _db.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
            {
                UserId = entity.Id,
                RoleId = string.Equals(Input.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User"
            });

            await _db.SaveChangesAsync();
            TempData["Msg"] = $"Saved changes for '{entity.UserName}'.";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostChangePassword(int id, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6 || newPassword != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password invalid or does not match confirmation.");
                return await OnGet(id);
            }

            var entity = await _db.Users.FindAsync(id);
            if (entity == null) return NotFound();

            entity.PasswordHash = PasswordHasher.Hash(newPassword);
            await _db.SaveChangesAsync();
            TempData["Msg"] = $"Password changed for '{entity.UserName}'.";
            return RedirectToPage("./Index");
        }

        private static EditUserInput MapFromEntity(Models.ApplicationUser u) => new()
        {
            Id = int.TryParse(u.Id, out var id) ? id : 0,
            Username = u.UserName ?? string.Empty,
            Email = u.Email,
            Role = "User",
            IsActive = u.IsActive
        };
    }
}
