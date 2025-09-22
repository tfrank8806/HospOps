// ============================================================================
// File: Models/AccessRequest.cs  (ADD NEW FILE)
// ============================================================================
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    /// <summary>Pending site access request submitted by a guest before account creation.</summary>
    public class AccessRequest
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string? MobilePhone { get; set; }

        [Required, StringLength(40)]
        public string PropertyCode { get; set; } = string.Empty; // MARSHA / INN code

        // Admin notes / status
        public bool Approved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        [StringLength(100)]
        public string? ApprovedBy { get; set; }
    }
}
