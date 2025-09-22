// File: Pages/WorkOrders/Details.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.WorkOrders
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly HospOpsContext _db;
        public DetailsModel(HospOpsContext db) => _db = db;

        public WorkOrder Item { get; private set; } = default!;

        public async Task OnGetAsync(int id)
        {
            Item = await _db.WorkOrders
                .Include(w => w.AssignedDepartment)
                .Include(w => w.StatusRef)
                .FirstOrDefaultAsync(w => w.Id == id) ?? new WorkOrder();
        }
    }
}
