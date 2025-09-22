// ============================================================================
// File: Models/LostItem.cs   (REPLACE ENTIRE FILE)
// Back-compat shims for: LostLocation, GuestName, DateReportedLost, ItemLost
// ============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospOps.Models
{
    public class LostItem
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Item { get; set; } = string.Empty;

        [StringLength(80)]
        public string? Location { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        // Optional department routing
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Guest-facing info (referenced by pages)
        [StringLength(120)]
        public string? GuestName { get; set; }

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
        public string ItemLost
        {
            get => Item;
            set => Item = value;
        }

        /// <summary>Alias for Location (legacy pages)</summary>
        [NotMapped]
        public string? LostLocation
        {
            get => Location;
            set => Location = value;
        }

        /// <summary>Alias that maps to CreatedAt (legacy pages)</summary>
        [NotMapped]
        public DateTime DateReportedLost
        {
            get => CreatedAt;
            set => CreatedAt = value;
        }
    }
}
