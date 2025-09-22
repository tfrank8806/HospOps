// ============================================================================
// File: Models/Department.cs   (REPLACE ENTIRE FILE)
// A proper EF entity (NOT an enum). One canonical Department model.
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        // Optional: visual tag for UI badges
        [StringLength(7)]
        public string? ColorHex { get; set; }  // e.g. "#0d6efd"

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; } = 0;

        // Optional multi-property support (keep nullable if not used yet)
        public int? PropertyId { get; set; }
    }
}
