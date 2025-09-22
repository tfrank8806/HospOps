// File: Pages/Logbook/Create.cshtml.cs
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
    public class CreateModel : PageModel
    {
        private readonly HospOpsContext _db;
        public CreateModel(HospOpsContext db) => _db = db;

        [BindProperty] public LogEntry Item { get; set; } = new();
        public SelectList DepartmentsSelect { get; private set; } = default!;
        public SelectList SeveritiesSelect { get; private set; } = default!;

        public async Task OnGetAsync() => await LoadListsAsync();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }

            Item.Date = DateTime.UtcNow.Date;
            Item.CreatedAt = DateTime.UtcNow;
            Item.CreatedBy = User.Identity?.Name ?? "system";

            _db.LogEntries.Add(Item);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index", new { Date = Item.Date.ToString("yyyy-MM-dd") });
        }

        private async Task LoadListsAsync()
        {
            DepartmentsSelect = new SelectList(await _db.Departments.Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder).ThenBy(d => d.Name).ToListAsync(), "Id", "Name");
            SeveritiesSelect = new SelectList(Enum.GetValues<Severity>().Select(s => new { Id = (int)s, Name = s.ToString() }), "Id", "Name");
        }
    }
}
