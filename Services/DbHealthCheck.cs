// ============================================================================
// File: Services/DbHealthCheck.cs  (ADD NEW FILE)
// ============================================================================
using System.Threading;
using System.Threading.Tasks;
using HospOps.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HospOps.Services
{
    /// <summary>Simple DB connectivity health check without extra packages.</summary>
    public sealed class DbHealthCheck : IHealthCheck
    {
        private readonly HospOpsContext _db;
        public DbHealthCheck(HospOpsContext db) => _db = db;

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _db.Database.CanConnectAsync(cancellationToken);
                return canConnect
                    ? HealthCheckResult.Healthy("Database reachable.")
                    : HealthCheckResult.Unhealthy("Database unreachable.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database check failed.", ex);
            }
        }
    }
}
