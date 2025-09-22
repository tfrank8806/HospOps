// ============================================================================
// File: Services/LogEntryArchiver.cs  (NEW FILE - ADD)
// ============================================================================
using HospOps.Data;
using HospOps.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace HospOps.Services
{
    /// <summary>Moves LogEntries older than 24 months into LogEntryArchives nightly (batched).</summary>
    public sealed class LogEntryArchiver : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<LogEntryArchiver> _log;

        public LogEntryArchiver(IServiceProvider sp, ILogger<LogEntryArchiver> log)
        {
            _sp = sp; _log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // small delay to avoid competing with startup work
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try { await RunOnce(stoppingToken); }
                catch (Exception ex) { _log.LogError(ex, "LogEntry archiver failed"); }

                // schedule ~02:15 UTC daily
                var next = DateTime.UtcNow.Date.AddDays(1).AddHours(2).AddMinutes(15);
                var delay = next - DateTime.UtcNow;
                if (delay < TimeSpan.FromMinutes(5)) delay = TimeSpan.FromHours(24);
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task RunOnce(CancellationToken ct)
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HospOpsContext>();

            var cutoff = DateTime.UtcNow.AddMonths(-24);
            const int batchSize = 1000;

            while (true)
            {
                var olds = await db.LogEntries.AsNoTracking()
                    .Where(e => e.CreatedAt < cutoff)
                    .OrderBy(e => e.CreatedAt)
                    .Take(batchSize)
                    .ToListAsync(ct);

                if (olds.Count == 0) break;

                var archives = olds.Select(e => new LogEntryArchive
                {
                    Date = e.Date,
                    Department = e.Department,
                    Title = e.Title,
                    Notes = e.Notes,
                    Severity = e.Severity,
                    CreatedBy = e.CreatedBy,
                    CreatedAt = e.CreatedAt
                }).ToList();

                using var tx = await db.Database.BeginTransactionAsync(ct);
                db.LogEntryArchives.AddRange(archives);
                db.LogEntries.RemoveRange(olds);
                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                _log.LogInformation("Archived {Count} log entries up to {LastDate:u}", olds.Count, olds.Last().CreatedAt);
            }
        }
    }
}