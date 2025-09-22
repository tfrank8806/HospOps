// ============================================================================
// File: Pages/Admin/Index.cshtml.cs  (ADD/REPLACE ENTIRE FILE)
// Admin Home dashboard: quick stats + navigation
// ============================================================================
using HospOps.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.Admin
{
    [Authorize(Roles = "Manager,Admin")]
    public class IndexModel : PageModel
    {
        private readonly HospOpsContext _db;
        public IndexModel(HospOpsContext db) => _db = db;

        public int DepartmentCount { get; private set; }
        public int WorkOrderTypeCount { get; private set; }
        public int WorkOrderStatusCount { get; private set; }
        public int CalendarCategoryCount { get; private set; }
        public int AccessRequestPending { get; private set; }

        public async Task OnGetAsync()
        {
            // Defensive: tables may not exist in early migrations
            DepartmentCount = await SafeCountAsync(() => _db.Departments.CountAsync());
            WorkOrderTypeCount = await SafeCountAsync(() => _db.WorkOrderTypes.CountAsync());
            WorkOrderStatusCount = await SafeCountAsync(() => _db.WorkOrderStatuses.CountAsync());
            CalendarCategoryCount = await SafeCountAsync(() => _db.CalendarCategories.CountAsync());
            AccessRequestPending = await SafeCountAsync(() => _db.AccessRequests.Where(a => !a.Approved).CountAsync());
        }

        private static async Task<int> SafeCountAsync(Func<Task<int>> query)
        {
            try { return await query(); }
            catch { return 0; } // table missing or not yet migrated
        }
    }
}
