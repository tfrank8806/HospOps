// ============================================================================
// File: Pages/Logbook/Edit.cshtml.cs  (REPLACE ENTIRE FILE)
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Logbook
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly HospOpsContext _db;
        public EditModel(HospOpsContext db) => _db = db;

        [BindProperty]
        public LogEntry Entry { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var e = await _db.LogEntries.FindAsync(id);
            if (e is null) return NotFound();
            Entry = e;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var entity = await _db.LogEntries.FirstOrDefaultAsync(x => x.Id == Entry.Id);
            if (entity is null) return NotFound();

            if (await TryUpdateModelAsync(
                entity, "Entry",
                e => e.Date, e => e.Department, e => e.Title, e => e.Notes, e => e.Severity))
            {
                if (!Enum.IsDefined(typeof(Severity), entity.Severity))
                    entity.Severity = Severity.Info;

                await _db.SaveChangesAsync();
                return RedirectToPage("./Index", new { date = entity.Date.ToString("yyyy-MM-dd") });
            }

            Entry = entity;
            return Page();
        }
    }
}