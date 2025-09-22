// File: Pages/WorkOrders/Index.cshtml.cs
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
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        public IndexModel(HospOpsContext db) => _db = db;

        public IList<WorkOrder> Items { get; private set; } = new List<WorkOrder>();
        public SelectList DepartmentsSelect { get; private set; } = default!;
        public SelectList StatusesSelect { get; private set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int? DepartmentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? StatusId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }

        public async Task OnGetAsync()
        {
            DepartmentsSelect = new SelectList(await _db.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder).ThenBy(d => d.Name)
                .ToListAsync(), "Id", "Name");

            StatusesSelect = new SelectList(await _db.WorkOrderStatuses
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder).ThenBy(s => s.Name)
                .ToListAsync(), "Id", "Name");

            var q = _db.WorkOrders
                .AsNoTracking()
                .Include(w => w.AssignedDepartment)
                .Include(w => w.StatusRef)
                .OrderByDescending(w => w.CreatedAt)
                .AsQueryable();

            if (DepartmentId is int did)
                q = q.Where(w => w.AssignedDepartmentId == did);

            if (StatusId is int sid)
                q = q.Where(w => w.StatusId == sid);

            if (!string.IsNullOrWhiteSpace(Keyword))
                q = q.Where(w => (w.Issue ?? "").Contains(Keyword!) || (w.Details ?? "").Contains(Keyword!));

            Items = await q.Take(200).ToListAsync();
        }
    }
}
