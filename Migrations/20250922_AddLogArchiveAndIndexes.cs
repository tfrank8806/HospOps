// ============================================================================
// File: Migrations/20250922_AddLogArchiveAndIndexes.cs   (ADD NEW FILE)
// ============================================================================
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospOps.Migrations
{
    /// <summary>
    /// Creates LogEntryArchives and adds daily-view indexes for fast paging/filtering.
    /// Safe for both SQLite and SQL Server providers.
    /// </summary>
    public partial class AddLogArchiveAndIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Create LogEntryArchives ---
            migrationBuilder.CreateTable(
                name: "LogEntryArchives",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Department = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 160, nullable: false),
                    Notes = table.Column<string>(maxLength: 4000, nullable: true),
                    Severity = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntryArchives", x => x.Id);
                });

            // --- Indexes on LogEntryArchives (for archive browsing if needed) ---
            migrationBuilder.CreateIndex(
                name: "IX_LogEntryArchives_Date_CreatedAt",
                table: "LogEntryArchives",
                columns: new[] { "Date", "CreatedAt" });

            // --- Indexes on LogEntries for daily paging + filters ---
            // (If these already exist locally, EF will no-op; otherwise they’ll be created.)
            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Date_CreatedAt",
                table: "LogEntries",
                columns: new[] { "Date", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Date_Severity_CreatedAt",
                table: "LogEntries",
                columns: new[] { "Date", "Severity", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Date_Department_CreatedAt",
                table: "LogEntries",
                columns: new[] { "Date", "Department", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop added indexes on LogEntries
            migrationBuilder.DropIndex(
                name: "IX_LogEntries_Date_CreatedAt",
                table: "LogEntries");

            migrationBuilder.DropIndex(
                name: "IX_LogEntries_Date_Severity_CreatedAt",
                table: "LogEntries");

            migrationBuilder.DropIndex(
                name: "IX_LogEntries_Date_Department_CreatedAt",
                table: "LogEntries");

            // Drop archive table (and its index with it)
            migrationBuilder.DropTable(
                name: "LogEntryArchives");
        }
    }
}
