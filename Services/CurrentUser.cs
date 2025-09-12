// File: Services/CurrentUser.cs
using System.Security.Claims;

namespace HospOps.Services;

public interface ICurrentUser
{
    string? UserId { get; }
    string? UserName { get; }
}

/// <summary>
/// Provides current authenticated user info to non-HTTP layers (e.g., DbContext).
/// Falls back to null when no HTTP context exists.
/// </summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;
    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public string? UserId => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _http.HttpContext?.User?.Identity?.Name;
}
