// ============================================================================
// File: Pages/Admin/AccessRequests/Index.cshtml.cs  (REPLACE ENTIRE FILE)
// Resilient to missing AccessRequests table.
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace HospOps.Pages.Admin.AccessRequests
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(HospOpsContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
        {
            _db = db; _userManager = um; _roleManager = rm;
        }

        public List<AccessRequest> Pending { get; private set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                Pending = await _db.AccessRequests
                    .AsNoTracking()
                    .Where(a => !a.Approved)
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 1 && ex.Message.Contains("no such table", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Err"] = "The AccessRequests table is missing. Apply migrations to create it.";
                Pending = new();
            }
            catch (Exception)
            {
                TempData["Err"] = "Failed to load access requests.";
                Pending = new();
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var req = await _db.AccessRequests.FirstOrDefaultAsync(x => x.Id == id && !x.Approved);
            if (req == null) return NotFound();

            var user = new ApplicationUser
            {
                UserName = req.Email,
                Email = req.Email,
                DisplayName = $"{req.FirstName} {req.LastName}",
                EmailConfirmed = true
            };

            var tempPwd = Guid.NewGuid().ToString("N") + "aA1!";
            var result = await _userManager.CreateAsync(user, tempPwd);
            if (!result.Succeeded)
            {
                TempData["Err"] = "Failed to create user: " + string.Join("; ", result.Errors.Select(e => e.Description));
                return RedirectToPage();
            }

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));
            await _userManager.AddToRoleAsync(user, "User");

            var contact = new Contact
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Phones = new List<ContactPhone>(),
                Emails = new List<ContactEmail> { new ContactEmail { Address = req.Email } }
            };
            if (!string.IsNullOrWhiteSpace(req.MobilePhone))
                contact.Phones.Add(new ContactPhone { Number = req.MobilePhone! });

            _db.Contacts.Add(contact);

            req.Approved = true;
            req.ApprovedAt = DateTime.UtcNow;
            req.ApprovedBy = User?.Identity?.Name;

            await _db.SaveChangesAsync();

            TempData["Msg"] = $"Approved and created user for {req.Email}.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var req = await _db.AccessRequests.FirstOrDefaultAsync(x => x.Id == id && !x.Approved);
            if (req == null) return NotFound();
            _db.AccessRequests.Remove(req);
            await _db.SaveChangesAsync();
            TempData["Msg"] = "Request rejected and removed.";
            return RedirectToPage();
        }
    }
}
