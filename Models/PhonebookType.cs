// ============================================================================
// File: Models/PhonebookType.cs  (ADD NEW FILE)
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class PhonebookType
    {
        public int Id { get; set; }

        [Required, StringLength(40)]
        public string Name { get; set; } = string.Empty; // e.g., "Employee", "Vendor", "Emergency"

        // CSS-compatible hex (#RRGGBB). Validation kept light here; UI should enforce format.
        [Required, StringLength(7)]
        public string ColorHex { get; set; } = "#6c757d";

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
