// File: HospOps/Models/WorkOrder.cs
using System.ComponentModel.DataAnnotations;

namespace HospOps.Models
{
    public enum WorkOrderStatus
    {
        Open = 0,
        InProgress = 1,
        Done = 2
    }

    public class WorkOrder
    {
        public int Id { get; set; }

        [Display(Name = "Room/Location"), Required, StringLength(50)]
        public string RoomOrLocation { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Department Department { get; set; }

        [Required]
        public Severity Severity { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

        [StringLength(2000)]
        public string? CompletionNotes { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ClosedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
