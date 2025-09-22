// ============================================================================
// File: Pages/Index.cshtml.cs   (REPLACE ENTIRE FILE)
// Home dashboard widgets (recent items)
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        public IndexModel(HospOpsContext db) => _db = db;

        public List<WorkOrder> RecentWorkOrders { get; private set; } = new();
        public List<FoundItem> RecentFound { get; private set; } = new();
        public List<LostItem> RecentLost { get; private set; } = new();
        public List<Event> UpcomingEvents { get; private set; } = new();

        public async Task OnGetAsync()
        {
            RecentWorkOrders = await _db.WorkOrders
                .AsNoTracking()
                .OrderByDescending(w => w.CreatedAt)
                .Take(5)
                .ToListAsync();

            RecentFound = await _db.FoundItems
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .ToListAsync();

            RecentLost = await _db.LostItems
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .ToListAsync();

            var now = DateTime.UtcNow.Date;
            UpcomingEvents = await _db.Events
                .AsNoTracking()
                .Where(e => e.StartDate >= now)
                .OrderBy(e => e.StartDate)
                .Take(5)
                .ToListAsync();
        }
    }
}
