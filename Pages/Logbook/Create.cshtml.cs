// File: Pages/Logbook/Create.cshtml.cs
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
    public LogEntry Entry { get; set; } = new LogEntry
    {
        Date = DateTime.UtcNow,
        Severity = Severity.Info
    };

    public void OnGet()
    {
        if (Entry.Date == default) Entry.Date = DateTime.UtcNow;
        Entry.Severity = Severity.Info;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        Entry.CreatedAt = DateTime.UtcNow;
        Entry.CreatedBy = User?.Identity?.Name ?? "system";

        _db.LogEntries.Add(Entry);
        await _db.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
