// File: HospOps/Pages/WorkOrders/Edit.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.WorkOrders
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly HospOpsContext _db;
        public EditModel(HospOpsContext db) => _db = db;

        [BindProperty]
        public WorkOrder Order { get; set; } = null!;

        public async Task<IActionResult> OnGet(int id)
        {
            var entity = await _db.WorkOrders.FirstOrDefaultAsync(w => w.Id == id);
            if (entity == null) return NotFound();
            Order = entity;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var entity = await _db.WorkOrders.FirstOrDefaultAsync(w => w.Id == id);
            if (entity == null) return NotFound();

            if (await TryUpdateModelAsync(entity, "Order",
                    w => w.RoomOrLocation, w => w.Description, w => w.Department,
                    w => w.Severity, w => w.DueDate, w => w.Status,
                    w => w.ClosedAt, w => w.CompletionNotes))
            {
                if (entity.Status == WorkOrderStatus.Done && entity.ClosedAt == null)
                    entity.ClosedAt = DateTime.Today;

                if (entity.Status != WorkOrderStatus.Done)
                    entity.ClosedAt = null;

                await _db.SaveChangesAsync();
                TempData["Msg"] = "Work order updated.";
                return RedirectToPage("./Index");
            }

            Order = entity;
            return Page();
        }
    }
}
