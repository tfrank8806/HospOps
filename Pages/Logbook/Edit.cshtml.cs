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
        public LogEntry Entry { get; set; } = null!;

        public async Task<IActionResult> OnGet(int id)
        {
            var entity = await _db.LogEntries.FindAsync(id);
            if (entity == null) return NotFound();
            Entry = entity;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var entity = await _db.LogEntries.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return NotFound();

            if (await TryUpdateModelAsync<LogEntry>(
                entity,
                "Entry",
                e => e.Date, e => e.Department, e => e.Title, e => e.Notes, e => e.Severity))
            {
                await _db.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            Entry = entity;
            return Page();
        }
    }
}
