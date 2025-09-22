// File: Pages/Logbook/Index.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Logbook
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        public IndexModel(HospOpsContext db) => _db = db;

        public IList<LogEntry> Items { get; private set; } = new List<LogEntry>();
        public SelectList DepartmentsSelect { get; private set; } = default!;
        public SelectList SeveritiesSelect { get; private set; } = default!;

        [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; } = DateTime.Today;

        [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
        public int? DepartmentId { get; set; }

        [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
        public Severity? Severity { get; set; }

        public async Task OnGetAsync()
        {
            DepartmentsSelect = new SelectList(await _db.Departments
                .Where(d => d.IsActive).OrderBy(d => d.SortOrder).ThenBy(d => d.Name).ToListAsync(), "Id", "Name");

            SeveritiesSelect = new SelectList(Enum.GetValues<Severity>().Select(s => new { Id = (int)s, Name = s.ToString() }), "Id", "Name");

            var day = (Date ?? DateTime.Today).Date;
            var next = day.AddDays(1);

            var q = _db.LogEntries.AsNoTracking().Where(l => l.Date >= day && l.Date < next);

            if (DepartmentId is int did) q = q.Where(l => l.DepartmentId == did);
            if (Severity is Severity sev) q = q.Where(l => l.Severity == sev);

            Items = await q.OrderByDescending(l => l.CreatedAt).ToListAsync();
        }
    }
}
