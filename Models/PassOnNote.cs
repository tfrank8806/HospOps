using System.ComponentModel.DataAnnotations;


namespace HospOps.Models
{
    /// <summary>
    /// Shift "Pass On" note to be reviewed by the next shift.
    /// </summary>
    public class PassOnNote
    {
        public int Id { get; set; }


        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;


        [Required]
        public Department Department { get; set; } = Department.Management; // keep consistent with site


        [Required, StringLength(160)]
        public string Title { get; set; } = string.Empty;


        [Required, StringLength(4000)]
        public string Message { get; set; } = string.Empty;


        [StringLength(100)]
        public string? CreatedBy { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}