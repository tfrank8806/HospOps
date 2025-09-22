// ============================================================================
// File: Pages/Logbook/Index.cshtml.cs  (REPLACE ENTIRE FILE)
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Logbook;

[Authorize]
public class IndexModel : PageModel
{
    private readonly HospOpsContext _db;
    public IndexModel(HospOpsContext db) => _db = db;

    public List<LogEntry> Entries { get; private set; } = new();

    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? Date { get; set; }
    [BindProperty(SupportsGet = true)] public Department? DepartmentFilter { get; set; }
    [BindProperty(SupportsGet = true)] public Severity? SeverityFilter { get; set; }

    public DateTime Day => (Date?.Date) ?? System.DateTime.Today;

    public async Task OnGetAsync()
    {
        IQueryable<LogEntry> q = _db.LogEntries.AsNoTracking();

        // one full day per page
        q = q.Where(e => e.Date.Date == Day);

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var t = $"%{Q.Trim()}%";
            q = q.Where(e =>
                (e.Title != null && EF.Functions.Like(e.Title, t)) ||
                (e.Notes != null && EF.Functions.Like(e.Notes, t)) ||
                (e.CreatedBy != null && EF.Functions.Like(e.CreatedBy, t)));
        }

        if (DepartmentFilter.HasValue)
            q = q.Where(e => e.Department == DepartmentFilter.Value);

        if (SeverityFilter.HasValue)
            q = q.Where(e => e.Severity == SeverityFilter.Value);

        Entries = await q
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public IActionResult OnGetPrevDay() =>
        RedirectToPage(new { date = Day.AddDays(-1).ToString("yyyy-MM-dd"), q = Q, DepartmentFilter, SeverityFilter });

    public IActionResult OnGetNextDay() =>
        RedirectToPage(new { date = Day.AddDays(1).ToString("yyyy-MM-dd"), q = Q, DepartmentFilter, SeverityFilter });

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!User.IsInRole("Admin")) return Forbid();
        var entity = await _db.LogEntries.FindAsync(id);
        if (entity is null) return NotFound();
        _db.LogEntries.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { date = Day.ToString("yyyy-MM-dd"), q = Q, DepartmentFilter, SeverityFilter });
    }
}