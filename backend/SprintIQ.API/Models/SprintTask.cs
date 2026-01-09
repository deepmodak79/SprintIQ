using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.Models;

public class SprintTask
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public int SprintId { get; set; }
    public virtual Sprint Sprint { get; set; } = null!;
    
    public int? AssigneeId { get; set; }
    public virtual User? Assignee { get; set; }
    
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    
    public int StoryPoints { get; set; } = 0;
    
    public int OrderIndex { get; set; } = 0;
    
    public bool IsBlocked { get; set; } = false;
    
    [MaxLength(500)]
    public string? BlockedReason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? StartedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    // Points awarded when completed
    public int PointsAwarded { get; set; } = 0;
    
    // Navigation properties
    public virtual ICollection<Blocker> Blockers { get; set; } = new List<Blocker>();
}

public enum TaskStatus
{
    Todo,
    InProgress,
    InReview,
    Done
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Critical
}
