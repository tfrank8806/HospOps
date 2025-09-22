// ============================================================================
// File: Models/WorkOrder.cs  (ADD THIS FILE; REPLACE ANY OLD VARIANTS)
// Backward-compatible Work Order with NotMapped shims for legacy Razor pages.
// ============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospOps.Models
{
    public class WorkOrder
    {
        public int Id { get; set; }

        // --- Canonical persisted fields ---
        [StringLength(40)]
        public string? Location { get; set; }              // e.g., "1204" or "Pool Pump Room"

        [Required, StringLength(200)]
        public string Issue { get; set; } = string.Empty;  // short title/summary

        [StringLength(4000)]
        public string? Details { get; set; }               // long description

        public int AssignedDepartmentId { get; set; }      // FK → Department
        public Department? AssignedDepartment { get; set; }

        public int? WorkOrderTypeId { get; set; }          // optional FK → WorkOrderType
        public WorkOrderType? WorkOrderType { get; set; }

        public int StatusId { get; set; }                  // FK → WorkOrderStatus
        public WorkOrderStatus? StatusRef { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? ClosedAt { get; set; }

        [StringLength(2000)]
        public string? CloseNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // --- NotMapped shims for legacy pages/views ---
        /// <summary>Legacy alias for Location.</summary>
        [NotMapped]
        public string? RoomOrLocation
        {
            get => Location;
            set => Location = value;
        }

        /// <summary>Legacy alias for Issue (short description).</summary>
        [NotMapped]
        public string? Description
        {
            get => Issue;
            set => Issue = value ?? string.Empty;
        }

        /// <summary>Legacy alias for CloseNotes.</summary>
        [NotMapped]
        public string? CompletionNotes
        {
            get => CloseNotes;
            set => CloseNotes = value;
        }

        /// <summary>Legacy projection: department id (enum-like) backed by FK.</summary>
        [NotMapped]
        public int Department
        {
            get => AssignedDepartmentId;
            set => AssignedDepartmentId = value;
        }

        /// <summary>Legacy projection: status name for quick display.</summary>
        [NotMapped]
        public string? Status => StatusRef?.Name;

        /// <summary>
        /// Legacy placeholder for severity (work orders don't persist severity; logs do).
        /// Provided only to keep old pages compiling; no storage.
        /// </summary>
        [NotMapped]
        public int? Severity { get; set; }
    }
}
