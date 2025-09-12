// File: HospOps/Pages/WorkOrders/Details.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.WorkOrders
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly HospOpsContext _db;
        public DetailsModel(HospOpsContext db) => _db = db;

        public WorkOrder? Order { get; private set; }

        public async Task<IActionResult> OnGet(int id)
        {
            Order = await _db.WorkOrders.FindAsync(id);
            if (Order == null) return NotFound();
            return Page();
        }
    }
}
