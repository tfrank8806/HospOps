// File: Pages/WorkOrders/Edit.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.WorkOrders
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly HospOpsContext _db;
        public EditModel(HospOpsContext db) => _db = db;

        [BindProperty]
        public WorkOrder Item { get; set; } = default!;

        public SelectList DepartmentsSelect { get; private set; } = default!;
        public SelectList StatusesSelect { get; private set; } = default!;
        public SelectList TypesSelect { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // why: avoid CS8601 by assigning to local then null-checking
            var item = await _db.WorkOrders
                .Include(w => w.AssignedDepartment)
                .Include(w => w.StatusRef)
                .Include(w => w.WorkOrderType)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (item is null) return NotFound();
            Item = item;

            await LoadListsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadListsAsync();
                return Page();
            }

            var existing = await _db.WorkOrders.FirstOrDefaultAsync(w => w.Id == Item.Id);
            if (existing is null) return NotFound();

            existing.Location = Item.Location;
            existing.Issue = Item.Issue;
            existing.Details = Item.Details;
            existing.AssignedDepartmentId = Item.AssignedDepartmentId;
            existing.WorkOrderTypeId = Item.WorkOrderTypeId;
            existing.StatusId = Item.StatusId;
            existing.DueDate = Item.DueDate;

            var status = await _db.WorkOrderStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == Item.StatusId);

            bool isClosed = status?.Name != null &&
                            status.Name.Equals("Done", StringComparison.OrdinalIgnoreCase);

            existing.ClosedAt = isClosed ? (existing.ClosedAt ?? DateTime.UtcNow) : null;
            existing.CloseNotes = Item.CloseNotes;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }

        private async Task LoadListsAsync()
        {
            var depts = await _db.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder).ThenBy(d => d.Name)
                .ToListAsync();
            DepartmentsSelect = new SelectList(depts, "Id", "Name", Item?.AssignedDepartmentId);

            var statuses = await _db.WorkOrderStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder).ThenBy(s => s.Name)
                .ToListAsync();
            StatusesSelect = new SelectList(statuses, "Id", "Name", Item?.StatusId);

            var types = await _db.WorkOrderTypes
                .OrderBy(t => t.Name)
                .ToListAsync();
            TypesSelect = new SelectList(types, "Id", "Name", Item?.WorkOrderTypeId);
        }
    }
}
