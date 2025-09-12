using HospOps.Data;
using HospOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospOps.Pages.Logbook
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly HospOpsContext _db;
        public DetailsModel(HospOpsContext db) => _db = db;

        public LogEntry Entry { get; private set; } = null!;

        public async Task<IActionResult> OnGet(int id)
        {
            var entity = await _db.LogEntries.FindAsync(id);
            if (entity == null) return NotFound();
            Entry = entity;
            return Page();
        }
    }
}
