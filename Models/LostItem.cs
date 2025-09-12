using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public class LostItem
    {
        public int Id { get; set; }

        [MaxLength(80)]
        public string? LostLocation { get; set; }

        public DateTime? GuestStayStart { get; set; }
        public DateTime? GuestStayEnd { get; set; }

        // Stored non-null; defaulted to today when empty on the form.
        public DateTime DateReportedLost { get; set; }

        [MaxLength(120)]
        public string? GuestName { get; set; }

        [MaxLength(40)]
        public string? Phone { get; set; }

        [MaxLength(200)]
        [EmailAddress] // Optional; only validated if provided
        public string? Email { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

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
