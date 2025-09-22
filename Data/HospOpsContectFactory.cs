// ============================================================================
// File: Data/HospOpsContectFactory.cs   (REPLACE ENTIRE FILE)
// NOTE: the filename in your error is "HospOpsContectFactory.cs" (with 'Contect').
// Keep that exact filename or rename both file and class consistently.
// ============================================================================
using System;
using System.IO;
using HospOps.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HospOps.Data
{
    /// <summary>
    /// Design-time factory for EF Core CLI (dotnet ef ...). Constructs HospOpsContext
    /// with a single DbContextOptions argument to match the runtime constructor.
    /// </summary>
    public sealed class HospOpsContectFactory : IDesignTimeDbContextFactory<HospOpsContext>
    {
        public HospOpsContext CreateDbContext(string[] args)
        {
            // Resolve content root to the project directory where appsettings*.json live.
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = config.GetConnectionString("DefaultConnection") ?? string.Empty;

            var optionsBuilder = new DbContextOptionsBuilder<HospOpsContext>();

            if (!string.IsNullOrWhiteSpace(cs) && cs.Contains("Server=", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlServer(cs);
            }
            else
            {
                // Fallback to a local SQLite DB under a writable folder for tooling.
                var home = Environment.GetEnvironmentVariable("HOME") ?? AppContext.BaseDirectory;
                var dbPath = Path.Combine(home, "data", "hospops.design.db");
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }

            return new HospOpsContext(optionsBuilder.Options);
        }
    }
}
