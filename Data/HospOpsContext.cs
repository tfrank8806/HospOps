// ============================================================================
// File: Data/HospOpsContext.cs   (REPLACE ENTIRE FILE)
// Ensures Department is an entity (not enum) and configures lookups.
// ============================================================================
using HospOps.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Data
{
    public class HospOpsContext : IdentityDbContext<ApplicationUser>
    {
        public HospOpsContext(DbContextOptions<HospOpsContext> options) : base(options) { }

        // --- Core/Lookups ---
        public DbSet<Department> Departments => Set<Department>();

        // --- Logbook ---
        public DbSet<LogEntry> LogEntries => Set<LogEntry>();
        public DbSet<LogEntryArchive> LogEntryArchives => Set<LogEntryArchive>();

        // --- Work Orders ---
        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        public DbSet<WorkOrderType> WorkOrderTypes => Set<WorkOrderType>();
        public DbSet<WorkOrderStatus> WorkOrderStatuses => Set<WorkOrderStatus>();

        // --- Lost & Found ---
        public DbSet<LostItem> LostItems => Set<LostItem>();
        public DbSet<FoundItem> FoundItems => Set<FoundItem>();
        public DbSet<ItemChangeLog> ItemChangeLogs => Set<ItemChangeLog>();

        // --- Phonebook ---
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<ContactPhone> ContactPhones => Set<ContactPhone>();
        public DbSet<ContactEmail> ContactEmails => Set<ContactEmail>();
        public DbSet<PhonebookType> PhonebookTypes => Set<PhonebookType>();

        // --- Calendar ---
        public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
        public DbSet<CalendarCategory> CalendarCategories => Set<CalendarCategory>();
        public DbSet<Event> Events => Set<Event>();

        // --- Pass On ---
        public DbSet<PassOnNote> PassOnNotes => Set<PassOnNote>();
        public DbSet<PassOnProperty> PassOnProperties => Set<PassOnProperty>();

        // --- Property data ---
        public DbSet<Floor> Floors => Set<Floor>();
        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<Room> Rooms => Set<Room>();

        // --- Access Requests ---
        public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Departments
            b.Entity<Department>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(80).IsRequired();
                e.HasIndex(x => x.IsActive);
                e.HasIndex(x => x.SortOrder);
                e.HasIndex(x => x.Name).IsUnique();
            });

            // Logbook
            b.Entity<LogEntry>(e =>
            {
                e.Property(x => x.Title).HasMaxLength(160).IsRequired();
                e.Property(x => x.Notes).HasMaxLength(4000);
                e.Property(x => x.CreatedBy).HasMaxLength(100);
                e.HasIndex(x => new { x.Date, x.CreatedAt });
                e.HasIndex(x => new { x.Date, x.Severity, x.CreatedAt });
                e.HasIndex(x => new { x.Date, x.Department, x.CreatedAt });
            });
            b.Entity<LogEntryArchive>(e =>
            {
                e.Property(x => x.Title).HasMaxLength(160).IsRequired();
                e.Property(x => x.Notes).HasMaxLength(4000);
                e.Property(x => x.CreatedBy).HasMaxLength(100);
                e.HasIndex(x => new { x.Date, x.CreatedAt });
            });

            // Floors
            b.Entity<Floor>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(40).IsRequired();
                e.Property(x => x.Description).HasMaxLength(100);
                e.HasIndex(x => x.SortOrder);
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            // RoomTypes
            b.Entity<RoomType>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(60).IsRequired();
                e.Property(x => x.Description).HasMaxLength(120);
                e.HasIndex(x => x.IsActive);
                e.HasIndex(x => x.SortOrder);
            });

            // Rooms
            b.Entity<Room>(e =>
            {
                e.Property(x => x.RoomNumber).HasMaxLength(20).IsRequired();
                e.HasIndex(x => x.RoomNumber).IsUnique();
                e.HasOne(x => x.Floor).WithMany(f => f.Rooms).HasForeignKey(x => x.FloorId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.RoomType).WithMany(t => t.Rooms).HasForeignKey(x => x.RoomTypeId).OnDelete(DeleteBehavior.SetNull);
                e.HasIndex(x => x.FloorId);
                e.HasIndex(x => x.RoomTypeId);
            });

            // PhonebookTypes
            b.Entity<PhonebookType>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(40).IsRequired();
                e.Property(x => x.ColorHex).HasMaxLength(7).IsRequired();
                e.HasIndex(x => x.IsActive);
                e.HasIndex(x => x.SortOrder);
                e.HasIndex(x => x.Name).IsUnique();
            });

            // WorkOrderStatus
            b.Entity<WorkOrderStatus>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(40).IsRequired();
                e.Property(x => x.ColorHex).HasMaxLength(7).IsRequired();
                e.HasIndex(x => x.SortOrder);
                e.HasIndex(x => x.IsActive);
                // Single default status enforced via migration unique partial index
            });

            // AccessRequest
            b.Entity<AccessRequest>(e =>
            {
                e.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
                e.Property(x => x.LastName).HasMaxLength(50).IsRequired();
                e.Property(x => x.Email).HasMaxLength(254).IsRequired();
                e.Property(x => x.PropertyCode).HasMaxLength(40).IsRequired();
                e.Property(x => x.ApprovedBy).HasMaxLength(100);
                e.HasIndex(x => new { x.Email, x.Approved });
                e.HasIndex(x => x.CreatedAt);
            });
        }
    }
}
