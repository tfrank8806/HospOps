// ============================================================================
// File: Pages/PassOn/Index.cshtml.cs   (REPLACE ENTIRE FILE)
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.PassOn
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        public IndexModel(HospOpsContext db) => _db = db;

        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }

        [BindProperty]
        public PassOnNote Note { get; set; } = new();

        public List<PassOnNote> Notes { get; private set; } = new();

        public DateTime Day => (Date?.Date) ?? System.DateTime.Today;

        public async Task OnGetAsync()
        {
            Notes = await _db.PassOnNotes
                .AsNoTracking()
                .Where(n => n.Date == Day)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            Note.Date = (Note.Date == default ? DateTime.UtcNow.Date : Note.Date.Date);
            Note.CreatedAt = DateTime.UtcNow;
            Note.CreatedBy = User?.Identity?.Name ?? "system";
            _db.PassOnNotes.Add(Note);
            await _db.SaveChangesAsync();
            return RedirectToPage(new { date = Note.Date.ToString("yyyy-MM-dd") });
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var entity = await _db.PassOnNotes.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null) return NotFound();

            if (await TryUpdateModelAsync(
                entity, "Note",
                n => n.Department, n => n.Title, n => n.Message))
            {
                entity.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return RedirectToPage(new { date = entity.Date.ToString("yyyy-MM-dd") });
            }

            await OnGetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!User.IsInRole("Admin")) return Forbid();
            var entity = await _db.PassOnNotes.FindAsync(id);
            if (entity is null) return NotFound();
            _db.PassOnNotes.Remove(entity);
            await _db.SaveChangesAsync();
            return RedirectToPage(new { date = entity.Date.ToString("yyyy-MM-dd") });
        }

        public IActionResult OnGetPrevDay() =>
            RedirectToPage(new { date = Day.AddDays(-1).ToString("yyyy-MM-dd") });

        public IActionResult OnGetNextDay() =>
            RedirectToPage(new { date = Day.AddDays(1).ToString("yyyy-MM-dd") });
    }
}
