using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.Models;

public class Blocker
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public int? TaskId { get; set; }
    public virtual SprintTask? Task { get; set; }
    
    public int ReportedById { get; set; }
    public virtual User ReportedBy { get; set; } = null!;
    
    public int? AssignedToId { get; set; }
    public virtual User? AssignedTo { get; set; }
    
    public int SprintId { get; set; }
    public virtual Sprint Sprint { get; set; } = null!;
    
    public BlockerStatus Status { get; set; } = BlockerStatus.Open;
    
    public BlockerSeverity Severity { get; set; } = BlockerSeverity.Medium;
    
    [MaxLength(1000)]
    public string? Resolution { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ResolvedAt { get; set; }
    
    // AI-suggested solution
    [MaxLength(1000)]
    public string? AiSuggestion { get; set; }
}

public enum BlockerStatus
{
    Open,
    InProgress,
    Resolved,
    Escalated
}

public enum BlockerSeverity
{
    Low,
    Medium,
    High,
    Critical
}
