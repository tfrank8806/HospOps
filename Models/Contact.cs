using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospOps.Models;

public class Contact
{
    public int Id { get; set; }

    [StringLength(60)] public string? FirstName { get; set; }
    [StringLength(60)] public string? LastName { get; set; }
    [StringLength(120)] public string? Company { get; set; }

    [StringLength(200)] public string? Address { get; set; }
    [Url, StringLength(300)] public string? Website { get; set; }

    [StringLength(4000)] public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<ContactPhone> Phones { get; set; } = new List<ContactPhone>();
    public ICollection<ContactEmail> Emails { get; set; } = new List<ContactEmail>();
}

public class ContactPhone
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public Contact? Contact { get; set; }

    [StringLength(40)] public string? Label { get; set; } // e.g. Mobile, Office
    [Phone, StringLength(40)] public string? Number { get; set; }
    public int SortOrder { get; set; }
}

public class ContactEmail
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public Contact? Contact { get; set; }

    [StringLength(40)] public string? Label { get; set; } // e.g. Work, Personal
    [EmailAddress, StringLength(200)] public string? Address { get; set; }
    public int SortOrder { get; set; }
}
