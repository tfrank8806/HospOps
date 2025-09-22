// File: Models/LogEntryArchive.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospOps.Models
{
    public class LogEntryArchive
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        [Required, StringLength(160)]
        public string Title { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Notes { get; set; }

        public Severity Severity { get; set; } = Severity.Info;

        // Persisted FK
        public int? DepartmentId { get; set; }

        // TEMP: Ignore at EF mapping time to unblock migrations/design-time
        [NotMapped]
        public Department? Department { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
