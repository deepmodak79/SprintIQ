using System.ComponentModel.DataAnnotations;
using SprintIQ.API.Models;
using TaskStatus = SprintIQ.API.Models.TaskStatus;

namespace SprintIQ.API.DTOs;

public class CreateTaskDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    [Required]
    public int SprintId { get; set; }
    
    public int? AssigneeId { get; set; }
    
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    
    public int StoryPoints { get; set; } = 0;
}

public class UpdateTaskDto
{
    [MaxLength(200)]
    public string? Title { get; set; }
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public int? AssigneeId { get; set; }
    
    public TaskStatus? Status { get; set; }
    
    public TaskPriority? Priority { get; set; }
    
    public int? StoryPoints { get; set; }
    
    public int? OrderIndex { get; set; }
    
    public bool? IsBlocked { get; set; }
    
    [MaxLength(500)]
    public string? BlockedReason { get; set; }
}

public class MoveTaskDto
{
    [Required]
    public TaskStatus NewStatus { get; set; }
    
    public int? NewOrderIndex { get; set; }
}

public class SprintTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SprintId { get; set; }
    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public string? AssigneeAvatar { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int StoryPoints { get; set; }
    public int OrderIndex { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int BlockerCount { get; set; }
}

public class KanbanBoardDto
{
    public List<SprintTaskDto> Todo { get; set; } = new();
    public List<SprintTaskDto> InProgress { get; set; } = new();
    public List<SprintTaskDto> InReview { get; set; } = new();
    public List<SprintTaskDto> Done { get; set; } = new();
}
