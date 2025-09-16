// File: Program.cs
using HospOps.Data;
using HospOps.Models;
using HospOps.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// Decide DB provider (Azure SQL in Prod if DefaultConnection looks like SQL Server; otherwise SQLite)
var env = builder.Environment;
var config = builder.Configuration;
var cs = config.GetConnectionString("DefaultConnection");
var looksLikeSqlServer = !string.IsNullOrWhiteSpace(cs) && cs.Contains("Server=", StringComparison.OrdinalIgnoreCase);

builder.Services.AddDbContext<HospOpsContext>(options =>
{
    if (!env.IsDevelopment() && looksLikeSqlServer)
    {
        // Azure SQL / SQL Server in Production
        options.UseSqlServer(cs);
    }
    else
    {
        // SQLite (Dev or no SQL connection provided). Use a writable path on App Service.
        var home = Environment.GetEnvironmentVariable("HOME") ?? AppContext.BaseDirectory;
        var dbPath = Path.Combine(home, "data", "hospops.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        options.UseSqlite($"Data Source={dbPath}");
    }
});

// Identity (use your existing ApplicationUser and Identity configuration)
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<HospOpsContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// --- Pipeline ---
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

// --- Migrate & Seed (safe; won't crash the process) ---
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
