// ============================================================================
// File: Models/CalendarEvent.cs   (REPLACE ENTIRE FILE)
// Add properties that Calendar pages expect: StartDate/EndDate/Recurring/EventName/EventType.
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class CalendarEvent
    {
        public int Id { get; set; }

        // Some pages may still bind by EventName/EventType; include both for compatibility.
        [Required, StringLength(160)]
        public string EventName { get; set; } = string.Empty;

        [StringLength(80)]
        public string? EventType { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        public bool Recurring { get; set; } = false;

        [StringLength(2000)]
        public string? Notes { get; set; }

        // Optional: keep a Title synonym if other pages use it.
        [StringLength(160)]
        public string? Title { get; set; }
    }
}
