// File: Data/HospOpsContext.cs
using HospOps.Models;
using HospOps.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospOps.Data;

public class HospOpsContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICurrentUser _currentUser;

    public HospOpsContext(DbContextOptions<HospOpsContext> options, ICurrentUser currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    // Core tables
    public DbSet<LogEntry> LogEntries => Set<LogEntry>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    // Lost & Found
    public DbSet<LostItem> LostItems => Set<LostItem>();
    public DbSet<FoundItem> FoundItems => Set<FoundItem>();
    public DbSet<ItemChangeLog> ItemChangeLogs => Set<ItemChangeLog>();

    // Phonebook
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ContactPhone> ContactPhones => Set<ContactPhone>();
    public DbSet<ContactEmail> ContactEmails => Set<ContactEmail>();

    // Pass On
    public DbSet<PassOnNote> PassOnNotes => Set<PassOnNote>();

    // Calendar (optional)
    public DbSet<Event>? Events => GetOptionalSet<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Phonebook relationships
        modelBuilder.Entity<Contact>(b =>
        {
            b.HasMany(c => c.Phones).WithOne(p => p.Contact!)
             .HasForeignKey(p => p.ContactId).OnDelete(DeleteBehavior.Cascade);
            b.HasMany(c => c.Emails).WithOne(e => e.Contact!)
             .HasForeignKey(e => e.ContactId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ContactPhone>(b =>
        {
            b.Property(p => p.Label).HasMaxLength(40);
            b.Property(p => p.Number).HasMaxLength(40);
        });

        modelBuilder.Entity<ContactEmail>(b =>
        {
            b.Property(e => e.Label).HasMaxLength(40);
            b.Property(e => e.Address).HasMaxLength(200);
        });

        modelBuilder.Entity<ItemChangeLog>(b =>
        {
            b.Property(x => x.ItemType).HasMaxLength(24).IsRequired();
            b.Property(x => x.Action).HasMaxLength(32).IsRequired();
            b.HasIndex(x => new { x.ItemType, x.ItemId, x.OccurredAt });
        });

        modelBuilder.Entity<PassOnNote>(b =>
        {
            b.Property(p => p.Title).HasMaxLength(160).IsRequired();
            b.Property(p => p.Message).HasMaxLength(4000).IsRequired();
            b.Property(p => p.CreatedBy).HasMaxLength(100);
        });
    }

    private DbSet<T>? GetOptionalSet<T>() where T : class
    {
        try { return Set<T>(); } catch { return null; }
    }

    private bool _inLogHook;

    public override int SaveChanges()
    {
        if (_inLogHook) return base.SaveChanges();
        var snapshot = SnapshotChanges();
        var result = base.SaveChanges();
        WriteLogEntries(snapshot).GetAwaiter().GetResult();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_inLogHook) return await base.SaveChangesAsync(cancellationToken);
        var snapshot = SnapshotChanges();
        var result = await base.SaveChangesAsync(cancellationToken);
        await WriteLogEntries(snapshot, cancellationToken);
        return result;
    }

    private sealed record ChangeRecord(string Kind, object Entity);

    private List<ChangeRecord> SnapshotChanges()
    {
        var list = new List<ChangeRecord>();
        foreach (var e in ChangeTracker.Entries())
        {
            if (e.Entity is LogEntry) continue;
            if (e.State is not (EntityState.Added or EntityState.Modified)) continue;

            var t = e.Entity.GetType();
            if (t == typeof(WorkOrder))
                list.Add(new ChangeRecord(e.State == EntityState.Added ? "WorkOrderCreated" : "WorkOrderUpdated", e.Entity));
            else if (t == typeof(LostItem))
                list.Add(new ChangeRecord(e.State == EntityState.Added ? "LostCreated" : "LostUpdated", e.Entity));
            else if (t == typeof(FoundItem))
                list.Add(new ChangeRecord(e.State == EntityState.Added ? "FoundCreated" : "FoundUpdated", e.Entity));
            else if (t == typeof(PassOnNote))
                list.Add(new ChangeRecord(e.State == EntityState.Added ? "PassOnCreated" : "PassOnUpdated", e.Entity));
            else if (t.Name.Contains("Event", StringComparison.OrdinalIgnoreCase))
                list.Add(new ChangeRecord(e.State == EntityState.Added ? "EventCreated" : "EventUpdated", e.Entity));
        }
        return list;
    }

    private async Task WriteLogEntries(List<ChangeRecord> items, CancellationToken ct = default)
    {
        if (items.Count == 0) return;
        _inLogHook = true;
        try
        {
            foreach (var item in items)
            {
                LogEntry? log = item.Kind switch
                {
                    "WorkOrderCreated" => BuildWorkOrderLog((WorkOrder)item.Entity, true),
                    "WorkOrderUpdated" => BuildWorkOrderLog((WorkOrder)item.Entity, false),
                    "LostCreated" => BuildLostLog((LostItem)item.Entity, true),
                    "LostUpdated" => BuildLostLog((LostItem)item.Entity, false),
                    "FoundCreated" => BuildFoundLog((FoundItem)item.Entity, true),
                    "FoundUpdated" => BuildFoundLog((FoundItem)item.Entity, false),
                    "PassOnCreated" => BuildPassOnLog((PassOnNote)item.Entity, true),
                    "PassOnUpdated" => BuildPassOnLog((PassOnNote)item.Entity, false),
                    "EventCreated" => BuildEventLog(item.Entity, true),
                    "EventUpdated" => BuildEventLog(item.Entity, false),
                    _ => null
                };
                if (log is not null) await LogEntries.AddAsync(log, ct);
            }
            await base.SaveChangesAsync(ct);
        }
        finally { _inLogHook = false; }
    }

    private static string Short(string? s, int max = 120)
        => string.IsNullOrWhiteSpace(s) ? string.Empty : (s = s.Trim()).Length <= max ? s : s.Substring(0, max - 1) + "…";

    private string ByDisplay() => string.IsNullOrWhiteSpace(_currentUser.UserName) ? "system" : _currentUser.UserName!;

    private LogEntry BuildWorkOrderLog(WorkOrder w, bool created)
    {
        var title = created
            ? $"WO #{w.Id} created • {w.Department} • {w.RoomOrLocation} • {w.Severity}"
            : $"WO #{w.Id} updated • {w.Department} • {w.RoomOrLocation} • {w.Severity}";

        string? dueSeg = null;
        try
        {
            var o = (object?)w.DueDate; // DateTime or DateTime?
            if (o is DateTime dt && dt != default) dueSeg = $"Due: {dt:yyyy-MM-dd}";
        }
        catch { }

        var notes = string.Join(" | ", new[] { Short(w.Description, 300), dueSeg, $"Status: {w.Status}" }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        return new LogEntry
        {
            Date = DateTime.UtcNow,
            Department = w.Department,
            Title = title,
            Notes = notes,
            Severity = w.Severity,
            CreatedBy = ByDisplay(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private LogEntry BuildLostLog(LostItem x, bool created)
    {
        var title = created ? $"Lost item #{x.Id} created" : $"Lost item #{x.Id} updated";
        var notes = string.Join(" | ", new[]
        {
            string.IsNullOrWhiteSpace(x.LostLocation) ? null : $"Loc: {x.LostLocation}",
            string.IsNullOrWhiteSpace(x.GuestName) ? null : $"Guest: {x.GuestName}",
            x.DateReportedLost is DateTime r && r != default ? $"Reported: {r:yyyy-MM-dd}" : null,
            Short(x.Description, 300)
        }.Where(s => !string.IsNullOrWhiteSpace(s)));

        return new LogEntry
        {
            Date = DateTime.UtcNow,
            Department = Department.FrontDesk,
            Title = title,
            Notes = notes,
            Severity = Severity.Low,
            CreatedBy = ByDisplay(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private LogEntry BuildFoundLog(FoundItem x, bool created)
    {
        var title = created ? $"Found item #{x.Id} created" : $"Found item #{x.Id} updated";
        var notes = string.Join(" | ", new[]
        {
            string.IsNullOrWhiteSpace(x.FoundLocation) ? null : $"Loc: {x.FoundLocation}",
            string.IsNullOrWhiteSpace(x.FoundBy) ? null : $"By: {x.FoundBy}",
            x.DateFound is DateTime f && f != default ? $"Found: {f:yyyy-MM-dd}" : null,
            Short(x.Description, 300)
        }.Where(s => !string.IsNullOrWhiteSpace(s)));

        return new LogEntry
        {
            Date = DateTime.UtcNow,
            Department = Department.FrontDesk,
            Title = title,
            Notes = notes,
            Severity = Severity.Low,
            CreatedBy = ByDisplay(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private LogEntry BuildPassOnLog(PassOnNote p, bool created)
    {
        var title = created ? "Pass On created" : "Pass On updated";
        var notes = string.Join(" | ", new[]
        {
            $"Dept: {p.Department}",
            string.IsNullOrWhiteSpace(p.Title) ? null : p.Title,
            Short(p.Message, 300)
        }.Where(s => !string.IsNullOrWhiteSpace(s)));

        return new LogEntry
        {
            Date = DateTime.UtcNow,
            Department = p.Department,
            Title = title,
            Notes = notes,
            Severity = Severity.Info,
            CreatedBy = ByDisplay(),
            CreatedAt = DateTime.UtcNow
        };
    }

    private LogEntry? BuildEventLog(object anyEvent, bool created)
    {
        var t = anyEvent.GetType();
        var id = t.GetProperty("Id")?.GetValue(anyEvent);
        var titleProp = t.GetProperty("Title")?.GetValue(anyEvent) as string;
        var start = t.GetProperty("Start")?.GetValue(anyEvent) ?? t.GetProperty("StartDate")?.GetValue(anyEvent);
        var end = t.GetProperty("End")?.GetValue(anyEvent) ?? t.GetProperty("EndDate")?.GetValue(anyEvent);

        var title = created ? $"Event #{id} created" : $"Event #{id} updated";

        var segs = new List<string>();
        if (!string.IsNullOrWhiteSpace(titleProp)) segs.Add(titleProp);
        if (start is DateTime sdt && sdt != default) segs.Add($"Start: {sdt:yyyy-MM-dd HH:mm}");
        if (end is DateTime edt && edt != default) segs.Add($"End: {edt:yyyy-MM-dd HH:mm}");
        var notes = string.Join(" | ", segs);

        return new LogEntry
        {
            Date = DateTime.UtcNow,
            Department = Department.Management,
            Title = title,
            Notes = notes,
            Severity = Severity.Low,
            CreatedBy = ByDisplay(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
