// ============================================================================
// File: Models/FoundItem.cs   (REPLACE ENTIRE FILE)
// Back-compat shims for: FoundLocation, FoundBy, DateFound, ItemFound
// ============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospOps.Models
{
    public class FoundItem
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Item { get; set; } = string.Empty;

        [StringLength(80)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        // Who found it (legacy pages expect FoundBy)
        [StringLength(120)]
        public string? FoundBy { get; set; }

        // Optional department routing
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public bool ReturnedToGuest { get; set; } = false;
        public DateTime? ReturnedAt { get; set; }
        [StringLength(100)] public string? ReturnedBy { get; set; }

        // System timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)] public string? CreatedBy { get; set; }
        [StringLength(100)] public string? UpdatedBy { get; set; }

        // ----------------------------
        // Back-compat alias properties
        // ----------------------------

        /// <summary>Alias for Item (legacy pages)</summary>
        [NotMapped]
        public string ItemFound
        {
            get => Item;
            set => Item = value;
        }

        /// <summary>Alias for Location (legacy pages)</summary>
        [NotMapped]
        public string? FoundLocation
        {
            get => Location;
            set => Location = value;
        }

        /// <summary>Alias that maps to CreatedAt (legacy pages)</summary>
        [NotMapped]
        public DateTime DateFound
        {
            get => CreatedAt;
            set => CreatedAt = value;
        }
    }
}
