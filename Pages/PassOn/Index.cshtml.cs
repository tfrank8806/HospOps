// File: Pages/PassOn/Index.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.PassOn
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        public IndexModel(HospOpsContext db) => _db = db;

        public List<PassOnNote> Recent { get; private set; } = new();
        public SelectList DepartmentsSelect { get; private set; } = default!;

        [BindProperty] public PassOnNote NewNote { get; set; } = new();

        public async Task OnGetAsync()
        {
            DepartmentsSelect = new SelectList(await _db.Departments.Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder).ThenBy(d => d.Name).ToListAsync(), "Id", "Name");

            Recent = await _db.PassOnNotes
                .AsNoTracking()
                .Include(p => p.Department)
                .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
                .Take(100)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            NewNote.CreatedAt = DateTime.UtcNow;
            NewNote.UpdatedAt = DateTime.UtcNow;
            NewNote.CreatedBy = User.Identity?.Name ?? "system";

            _db.PassOnNotes.Add(NewNote);
            await _db.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
