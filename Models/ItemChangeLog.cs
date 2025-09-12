using System.ComponentModel.DataAnnotations;


namespace HospOps.Models;


public class ItemChangeLog
{
    public int Id { get; set; }


    [Required, MaxLength(24)]
    public string ItemType { get; set; } = string.Empty; // "Lost" | "Found"


    public int ItemId { get; set; } // PK of LostItem/FoundItem


    [Required, MaxLength(32)]
    public string Action { get; set; } = string.Empty; // Created/Updated/Returned/Claimed


    [MaxLength(450)] public string? UserId { get; set; }
    [MaxLength(256)] public string? UserName { get; set; }


    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? ChangeSummary { get; set; }

    // Add this property to match usage in Index.cshtml.cs
    public DateTime ChangedAt { get; set; }
}