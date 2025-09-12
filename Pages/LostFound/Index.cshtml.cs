// File: Pages/LostFound/Index.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.LostFound;

[Authorize]
public class IndexModel : PageModel
{
    private readonly HospOpsContext _db;
    public IndexModel(HospOpsContext db) => _db = db;

    public List<LostItem> Lost { get; private set; } = new();
    public List<FoundItem> Found { get; private set; } = new();

    [BindProperty] public LostItem NewLost { get; set; } = new() { DateReportedLost = DateTime.UtcNow };
    [BindProperty] public FoundItem NewFound { get; set; } = new() { DateFound = DateTime.UtcNow };

    public async Task OnGetAsync()
    {
        Lost = await _db.LostItems.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt).Take(200).ToListAsync();

        Found = await _db.FoundItems.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt).Take(200).ToListAsync();
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostCreateLostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }
        NewLost.CreatedAt = DateTime.UtcNow;
        _db.LostItems.Add(NewLost);
        await _db.SaveChangesAsync(); // auto-log
        return RedirectToPage();
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostCreateFoundAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }
        NewFound.CreatedAt = DateTime.UtcNow;
        _db.FoundItems.Add(NewFound);
        await _db.SaveChangesAsync(); // auto-log
        return RedirectToPage();
    }

    // --- Admin deletes ---
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostDeleteLostAsync(int id)
    {
        if (!User.IsInRole("Admin")) return Forbid();
        var entity = await _db.LostItems.FindAsync(id);
        if (entity is null) return NotFound();
        _db.LostItems.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostDeleteFoundAsync(int id)
    {
        if (!User.IsInRole("Admin")) return Forbid();
        var entity = await _db.FoundItems.FindAsync(id);
        if (entity is null) return NotFound();
        _db.FoundItems.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
