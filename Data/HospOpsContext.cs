// ============================================================================
// File: Data/HospOpsContext.cs   (REPLACE ENTIRE FILE)
// ============================================================================
using HospOps.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Data
{
    public class HospOpsContext : IdentityDbContext<ApplicationUser>
    {
        public HospOpsContext(DbContextOptions<HospOpsContext> options) : base(options) { }

        // --- Logbook ---
        public DbSet<LogEntry> LogEntries => Set<LogEntry>();
        public DbSet<LogEntryArchive> LogEntryArchives => Set<LogEntryArchive>();

        // --- Work Orders ---
        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        // If WorkOrderType/Status are enums in your project, keep them out of DbSet.

        // --- Lost & Found ---
        public DbSet<LostItem> LostItems => Set<LostItem>();
        public DbSet<FoundItem> FoundItems => Set<FoundItem>();
        public DbSet<ItemChangeLog> ItemChangeLogs => Set<ItemChangeLog>();

        // --- Phonebook ---
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<ContactPhone> ContactPhones => Set<ContactPhone>();
        public DbSet<ContactEmail> ContactEmails => Set<ContactEmail>();

        // --- Calendar ---
        public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
        public DbSet<CalendarCategory> CalendarCategories => Set<CalendarCategory>();
        // Pages reference _db.Events of type HospOps.Data.Event
        public DbSet<Event> Events => Set<Event>();

        // --- Pass On ---
        public DbSet<PassOnNote> PassOnNotes => Set<PassOnNote>();
        public DbSet<PassOnProperty> PassOnProperties => Set<PassOnProperty>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Logbook indexes for "one day per page" + filters
            modelBuilder.Entity<LogEntry>(b =>
            {
                b.Property(x => x.Title).HasMaxLength(160).IsRequired();
                b.Property(x => x.Notes).HasMaxLength(4000);
                b.Property(x => x.CreatedBy).HasMaxLength(100);

                b.HasIndex(x => new { x.Date, x.CreatedAt });
                b.HasIndex(x => new { x.Date, x.Severity, x.CreatedAt });
                b.HasIndex(x => new { x.Date, x.Department, x.CreatedAt });
            });

            modelBuilder.Entity<LogEntryArchive>(b =>
            {
                b.Property(x => x.Title).HasMaxLength(160).IsRequired();
                b.Property(x => x.Notes).HasMaxLength(4000);
                b.Property(x => x.CreatedBy).HasMaxLength(100);

                b.HasIndex(x => new { x.Date, x.CreatedAt });
            });
        }
    }
}
