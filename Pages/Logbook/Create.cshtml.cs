// ============================================================================
// File: Pages/Logbook/Create.cshtml.cs  (REPLACE ENTIRE FILE)
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Logbook;

[Authorize]
public class CreateModel : PageModel
{
    private readonly HospOpsContext _db;
    public CreateModel(HospOpsContext db) => _db = db;

    [BindProperty]
    public LogEntry Entry { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // why: guarantee safe default even if UI tampered/missing
        if (!Enum.IsDefined(typeof(Severity), Entry.Severity))
            Entry.Severity = Severity.Info;

        Entry.Date = (Entry.Date == default ? DateTime.UtcNow.Date : Entry.Date.Date);
        Entry.CreatedAt = DateTime.UtcNow;
        Entry.CreatedBy = User?.Identity?.Name ?? "system";

        _db.LogEntries.Add(Entry);
        await _db.SaveChangesAsync();
        return RedirectToPage("./Index", new { date = Entry.Date.ToString("yyyy-MM-dd") });
    }
}