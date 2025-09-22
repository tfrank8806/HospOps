// ============================================================================
// File: Models/RoomType.cs  (ADD NEW FILE)
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; } = string.Empty; // e.g., "King", "Double Queen", "Suite"

        [StringLength(120)]
        public string? Description { get; set; }

        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
