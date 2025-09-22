// ============================================================================
// File: Models/LogEntryArchive.cs  (NEW FILE - ADD)
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class LogEntryArchive
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public Department Department { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Notes { get; set; }

        public Severity Severity { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}