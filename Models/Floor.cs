// ============================================================================
// File: Models/Floor.cs  (ADD NEW FILE)
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class Floor
    {
        public int Id { get; set; }

        [Required, StringLength(40)]
        public string Name { get; set; } = string.Empty; // e.g., "1", "2", "Lobby", "PH"

        [StringLength(100)]
        public string? Description { get; set; }

        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
