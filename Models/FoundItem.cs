using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class FoundItem
    {
        public int Id { get; set; }

        [MaxLength(80)]
        public string? FoundLocation { get; set; }

        [MaxLength(120)]
        public string? FoundBy { get; set; }

        // Stored non-null; defaulted to today when empty on the form.
        public DateTime DateFound { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(120)]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
