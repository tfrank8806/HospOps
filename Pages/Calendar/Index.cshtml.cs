using HospOps.Data;
using HospOps.Models; // <-- Add this using directive
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace HospOps.Pages.Calendar;

public class IndexModel : PageModel
{
    private readonly HospOpsContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(HospOpsContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
        NewEvent = new Event
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today
        };
    }

    [BindProperty]
    public Event NewEvent { get; set; } = new();

    public List<Event> Events { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (_context.Events == null)
        {
            Events = new List<Event>();
            return;
        }

        Events = await _context.Events
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            Events = _context.Events == null
                ? new List<Event>()
                : await _context.Events.ToListAsync();
            return Page();
        }

        if (_context.Events == null)
        {
            // Handle the null case appropriately, e.g., log error or return error page
            ModelState.AddModelError(string.Empty, "Events DbSet is not available.");
            Events = new List<Event>();
            return Page();
        }

        _context.Events.Add(NewEvent);
        await _context.SaveChangesAsync();

        // Log creation (set ItemId after SaveChanges so NewEvent.Id is populated)
        var log = new ItemChangeLog
        {
            ItemType = "Event",
            ItemId = NewEvent.Id,
            Action = "Created",
            ChangeSummary = $"Event '{NewEvent.EventName}' was created. Type: {NewEvent.EventType}, Start: {NewEvent.StartDate:d}, End: {NewEvent.EndDate:d}, Recurring: {(NewEvent.Recurring ? "Yes" : "No")}, Notes: {NewEvent.Notes}",
            ChangedAt = DateTime.UtcNow,
            OccurredAt = DateTime.UtcNow // <-- Add this if your logbook expects it
        };
        _context.ItemChangeLogs.Add(log);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (_context.Events == null)
        {
            // Handle the null case appropriately, e.g., log error or return error page
            ModelState.AddModelError(string.Empty, "Events DbSet is not available.");
            Events = new List<Event>();
            return Page();
        }

        var ev = await _context.Events.FindAsync(id);
        if (ev != null)
        {
            // Log deletion
            var log = new ItemChangeLog
            {
                ItemType = "Event",
                ItemId = ev.Id,
                Action = "Deleted",
                ChangeSummary = $"Event '{ev.EventName}' was deleted. Type: {ev.EventType}, Start: {ev.StartDate:d}, End: {ev.EndDate:d}, Recurring: {(ev.Recurring ? "Yes" : "No")}, Notes: {ev.Notes}",
                ChangedAt = DateTime.UtcNow,
                OccurredAt = DateTime.UtcNow // <-- Add this if your logbook expects it
            };
            _context.ItemChangeLogs.Add(log);

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    // Add similar handlers for Edit/Update as needed
}