// ============================================================================
// File: Pages/Calendar/Index.cshtml.cs   (REPLACE ENTIRE FILE)
// Queries HospOps.Data.Event and filters by selected date (one day per page)
// ============================================================================
using HospOps.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Calendar
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        public IndexModel(HospOpsContext db) => _db = db;

        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }

        public List<Event> Events { get; private set; } = new();

        public DateTime Day => (Date?.Date) ?? System.DateTime.Today;

        public async Task OnGetAsync()
        {
            var start = Day;
            var end = Day.AddDays(1);

            // Include events that start on the day OR span into the day
            Events = await _db.Events
                .AsNoTracking()
                .Where(e =>
                    (e.StartDate >= start && e.StartDate < end) ||
                    (e.StartDate < start && (e.EndDate == null || e.EndDate >= start)))
                .OrderBy(e => e.StartDate)
                .ToListAsync();
        }

        public IActionResult OnGetPrevDay() =>
            RedirectToPage(new { date = Day.AddDays(-1).ToString("yyyy-MM-dd") });

        public IActionResult OnGetNextDay() =>
            RedirectToPage(new { date = Day.AddDays(1).ToString("yyyy-MM-dd") });
    }
}
