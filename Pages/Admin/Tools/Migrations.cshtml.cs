// ============================================================================
// File: Pages/Admin/Tools/Migrations.cshtml.cs  (ADD NEW FILE)
// Admin-only page to apply migrations from the website.
// ============================================================================
using HospOps.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Admin.Tools
{
    [Authorize(Roles = "Admin")]
    public class MigrationsModel : PageModel
    {
        private readonly HospOpsContext _db;
        public List<string> Pending { get; private set; } = new();

        public MigrationsModel(HospOpsContext db) => _db = db;

        public async Task OnGetAsync()
        {
            Pending = (await _db.Database.GetPendingMigrationsAsync()).ToList();
        }

        public async Task<IActionResult> OnPostApplyAsync()
        {
            await _db.Database.MigrateAsync();
            TempData["Msg"] = "Migrations applied.";
            return RedirectToPage();
        }
    }
}
