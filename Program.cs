// ============================================================================
// File: Program.cs  (REPLACE ENTIRE FILE)
// ============================================================================
using System.IO;
using HospOps.Data;
using HospOps.Models;
using HospOps.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Persist DataProtection keys (avoid auth cookie invalidation on restarts/scale-out)
var home = Environment.GetEnvironmentVariable("HOME") ?? AppContext.BaseDirectory;
var keysPath = Path.Combine(home, "data", "keys");
Directory.CreateDirectory(keysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("HospOps");

// Database: SQL Server if conn string contains "Server=", else SQLite under %HOME%/data
var cs = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
builder.Services.AddDbContext<HospOpsContext>(options =>
{
    if (cs.Contains("Server=", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(cs);
    }
    else
    {
        var dbPath = Path.Combine(home, "data", "hospops.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        options.UseSqlite($"Data Source={dbPath}");
    }
});

// Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<HospOpsContext>();

builder.Services.AddRazorPages();

// Health checks (custom DB check; no EF health-check package required)
builder.Services.AddHealthChecks()
    .AddCheck<DbHealthCheck>("db", failureStatus: HealthStatus.Unhealthy);

// Hosted service: nightly archiving of old LogEntries
builder.Services.AddHostedService<LogEntryArchiver>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

// Safe startup migrate/seed
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<HospOpsContext>();
        db.Database.Migrate();
        await IdentitySeed.EnsureSeedAsync(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Startup migration/seed failed");
    }
}

app.Run();
