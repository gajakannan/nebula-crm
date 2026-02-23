namespace Nebula.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Normal";
    public DateTime? DueDate { get; set; }
    public string AssignedTo { get; set; } = default!;
    public string? LinkedEntityType { get; set; }
    public Guid? LinkedEntityId { get; set; }
    public DateTime? CompletedAt { get; set; }
}
