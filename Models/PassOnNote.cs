// File: Models/PassOnNote.cs
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class PassOnNote
    {
        public int Id { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Message { get; set; }

        // Department changed from enum → entity FK
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Optional: author
        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }
}
