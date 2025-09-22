// File: Pages/Logbook/Edit.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Logbook
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly HospOpsContext _db;
        public EditModel(HospOpsContext db) => _db = db;

        // Canonical property used by code-behind
        [BindProperty]
        public LogEntry Entry { get; set; } = default!;

        // Aliases to satisfy the existing .cshtml view bindings
        public LogEntry Item
        {
            get => Entry;
            set => Entry = value;
        }

        public SelectList DepartmentsSelect { get; private set; } = default!;
        public SelectList SeveritiesSelect { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var e = await _db.LogEntries.FirstOrDefaultAsync(x => x.Id == id);
            if (e is null) return NotFound();

            Entry = e;

            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            var existing = await _db.LogEntries.FirstOrDefaultAsync(x => x.Id == Entry.Id);
            if (existing is null) return NotFound();

            existing.Date = Entry.Date;
            existing.Title = Entry.Title;
            existing.Notes = Entry.Notes;
            existing.Severity = Entry.Severity;
            existing.DepartmentId = Entry.DepartmentId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        private async Task LoadDropdownsAsync()
        {
            var depts = await _db.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder).ThenBy(d => d.Name)
                .ToListAsync();

            DepartmentsSelect = new SelectList(depts, "Id", "Name", Entry?.DepartmentId);

            // Build severities list expected by the view
            var severities = Enum.GetValues(typeof(Severity))
                .Cast<Severity>()
                .Select(s => new { Id = (int)s, Name = s.ToString() })
                .ToList();

            SeveritiesSelect = new SelectList(severities, "Id", "Name", (int?)Entry?.Severity);
        }
    }
}
