// ============================================================================
// File: Services/AccessRequestSeed.cs  (REPLACE ENTIRE FILE)
// ============================================================================
using System.Threading.Tasks;
using HospOps.Data;
using HospOps.Models;
using Microsoft.EntityFrameworkCore;

// Provider-specific exceptions
#if NET8_0_OR_GREATER
using Microsoft.Data.Sqlite;
#endif

namespace HospOps.Services
{
    public static class AccessRequestSeed
    {
        public static async Task EnsureAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HospOpsContext>();

            // Try to ensure DB is at latest before seeding
            var pending = await db.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                await db.Database.MigrateAsync();
            }

            try
            {
                // If table doesn't exist yet, this query would throw for SQLite
                var hasAny = await db.AccessRequests.AsNoTracking().AnyAsync();
                if (!hasAny)
                {
                    db.AccessRequests.Add(new AccessRequest
                    {
                        FirstName = "Alex",
                        LastName = "Example",
                        Email = "alex@example.com",
                        MobilePhone = "555-0100",
                        PropertyCode = "TEST01",
                        Approved = false,
                        CreatedAt = DateTime.UtcNow
                    });

                    await db.SaveChangesAsync();
                }
            }
#if NET8_0_OR_GREATER
            catch (SqliteException ex) when (ex.SqliteErrorCode == 1 && ex.Message.Contains("no such table", StringComparison.OrdinalIgnoreCase))
            {
                // Table isn't created yet (e.g., migration not included or different context); skip silently.
            }
#endif
            catch
            {
                // Do not fail app startup on seeding; log-and-skip pattern is safer for first boot.
                // Consider adding structured logging here if desired.
            }
        }
    }
}
