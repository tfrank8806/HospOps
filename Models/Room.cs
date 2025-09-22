// ============================================================================
// File: Models/Room.cs  (ADD NEW FILE)
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        public int FloorId { get; set; }
        public Floor? Floor { get; set; }

        public int? RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }

        // Optional: coordinates on layout (pixels or %; to be filled by layout editor)
        public int? PosX { get; set; }
        public int? PosY { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        public bool IsOutOfOrder { get; set; } = false;

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
