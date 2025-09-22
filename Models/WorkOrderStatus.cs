// ============================================================================
// File: Models/WorkOrderStatus.cs   (REPLACE ENTIRE FILE; keep ONLY this version)
// Adds ColorHex + IsDefault to support color coding and default status.
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class WorkOrderStatus
    {
        public int Id { get; set; }

        [Required, StringLength(40)]
        public string Name { get; set; } = string.Empty;

        // CSS hex color (#RRGGBB)
        [Required, StringLength(7)]
        public string ColorHex { get; set; } = "#6c757d"; // why: list color-coding

        public bool IsDefault { get; set; } = false;      // why: default for new WOs
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
