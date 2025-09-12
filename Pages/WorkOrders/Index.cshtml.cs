// File: Pages/WorkOrders/Index.cshtml.cs
using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Pages.WorkOrders;

[Authorize]
public class IndexModel : PageModel
{
    private readonly HospOpsContext _db;
    public IndexModel(HospOpsContext db) => _db = db;

    public List<WorkOrder> Orders { get; private set; } = new();

    [BindProperty]
    public WorkOrder NewOrder { get; set; } = new()
    {
        Department = Department.Maintenance,
        Severity = Severity.Info,
        Status = WorkOrderStatus.Open,
        DueDate = default
    };

    public async Task OnGetAsync()
    {
        Orders = await _db.WorkOrders
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(200)
            .ToListAsync();
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        NewOrder.CreatedAt = DateTime.UtcNow;
        NewOrder.UpdatedAt = DateTime.UtcNow;

        _db.WorkOrders.Add(NewOrder);
        await _db.SaveChangesAsync(); // Will auto-write a LogEntry via HospOpsContext
        return RedirectToPage();
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!User.IsInRole("Admin")) return Forbid();

        var entity = await _db.WorkOrders.FindAsync(id);
        if (entity is null) return NotFound();

        _db.WorkOrders.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
