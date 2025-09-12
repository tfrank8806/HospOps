using System.ComponentModel.DataAnnotations;

namespace HospOps.Data;

public class Event
{
    public int Id { get; set; }

    [Required]
    public string EventName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    public string EventType { get; set; } = "Other"; // Group/Meeting/Other

    [DataType(DataType.Time)]
    public TimeSpan? StartTime { get; set; }

    [DataType(DataType.Time)]
    public TimeSpan? EndTime { get; set; }

    public string? Notes { get; set; }

    public bool Recurring { get; set; }
}