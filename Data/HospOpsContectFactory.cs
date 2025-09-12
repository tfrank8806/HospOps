// File: Data/HospOpsContextFactory.cs
// Design-time factory so EF Core tools can create HospOpsContext during migrations.
// This avoids issues caused by the constructor dependency on ICurrentUser.

using HospOps.Models;
using HospOps.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HospOps.Data;

public sealed class HospOpsContextFactory : IDesignTimeDbContextFactory<HospOpsContext>
{
    private sealed class DesignTimeCurrentUser : ICurrentUser
    {
        public string? UserId => null;           // no auth at design time
        public string? UserName => "design-time"; // just for log text if used
    }

    public HospOpsContext CreateDbContext(string[] args)
    {
        // Load configuration to read the same connection string used at runtime
        var basePath = Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var conn = config.GetConnectionString("DefaultConnection") ?? "Data Source=hospops.db";

        var optionsBuilder = new DbContextOptionsBuilder<HospOpsContext>();
        optionsBuilder.UseSqlite(conn);

        // Provide a fake current user so the context can be constructed
        return new HospOpsContext(optionsBuilder.Options, new DesignTimeCurrentUser());
    }
}
