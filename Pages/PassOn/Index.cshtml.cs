// =============================================
// File: Pages/PassOn/Index.cshtml.cs (NEW)
// =============================================
using DocumentFormat.OpenXml.Math;
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
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace HospOps.Pages.PassOn;

[Authorize]
[ValidateAntiForgeryToken]
public class IndexModel : PageModel
{
    private readonly HospOpsContext _db;
    public IndexModel(HospOpsContext db) => _db = db;

    public List<PassOnNote> Notes { get; private set; } = new();

    [BindProperty]
    public PassOnNote NewNote { get; set; } = new()
    {
        Date = DateTime.UtcNow,
        Department = Department.Management
    };

    public async Task OnGetAsync()
    {
        Notes = await _db.PassOnNotes
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.CreatedAt)
            .Take(200)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }
        NewNote.CreatedAt = DateTime.UtcNow;
        NewNote.UpdatedAt = DateTime.UtcNow;
        NewNote.CreatedBy = User?.Identity?.Name ?? "system";
        _db.PassOnNotes.Add(NewNote);
        await _db.SaveChangesAsync(); // auto-log will fire
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!User.IsInRole("Admin")) return Forbid(); // only admins
        var note = await _db.PassOnNotes.FindAsync(id);
        if (note is null) return NotFound();
        _db.PassOnNotes.Remove(note);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}