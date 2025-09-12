// File: Models/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HospOps.Models;

/// <summary>
/// Use ASP.NET Core Identity. Inherits from IdentityUser (string key) and
/// adds a couple optional profile fields. Role membership is handled via
/// Identity roles (not a string property).
/// </summary>
public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? DisplayName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
