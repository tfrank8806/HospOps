// File: Models/LogEntry.cs
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class LogEntry
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public Department Department { get; set; } = default;

        [Required, StringLength(160)]
        public string Title { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Notes { get; set; }

        [Required]
        public Severity Severity { get; set; } = Severity.Info;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
