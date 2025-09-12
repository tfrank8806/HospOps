// =============================================
// File: Pages/Logbook/Index.cshtml.cs  (UPDATED: admin delete)
// =============================================
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;

namespace HospOps.Pages.Logbook;

[Authorize]
[ValidateAntiForgeryToken]
public class IndexModel : PageModel
{
    private readonly HospOpsContext _db;
    public IndexModel(HospOpsContext db) => _db = db;

    public List<LogEntry> Entries { get; private set; } = new();

    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? Date { get; set; }
    [BindProperty(SupportsGet = true)] public Department? DepartmentFilter { get; set; }
    [BindProperty(SupportsGet = true)] public Severity? SeverityFilter { get; set; }

    public async Task OnGetAsync()
    {
        if (!Date.HasValue) Date = System.DateTime.Today;

        IQueryable<LogEntry> q = _db.LogEntries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Q))
        {
            var t = $"%{Q.Trim()}%";
            q = q.Where(e => (e.Title != null && EF.Functions.Like(e.Title, t))
                          || (e.Notes != null && EF.Functions.Like(e.Notes, t))
                          || (e.CreatedBy != null && EF.Functions.Like(e.CreatedBy, t)));
        }
        if (Date.HasValue)
        {
            var day = Date.Value.Date;
            q = q.Where(e => e.Date.Date == day);
        }
        if (DepartmentFilter.HasValue)
        {
            var dep = DepartmentFilter.Value;
            q = q.Where(e => e.Department == dep);
        }
        if (SeverityFilter.HasValue)
        {
            var sev = SeverityFilter.Value;
            q = q.Where(e => e.Severity == sev);
        }

        Entries = await q
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .Take(500)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!User.IsInRole("Admin")) return Forbid();
        var entity = await _db.LogEntries.FindAsync(id);
        if (entity is null) return NotFound();
        _db.LogEntries.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}