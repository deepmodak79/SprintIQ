using System.ComponentModel.DataAnnotations;
using SprintIQ.API.Models;

namespace SprintIQ.API.DTOs;

public class CreateBlockerDto
{
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public int? TaskId { get; set; }
    
    [Required]
    public int SprintId { get; set; }
    
    public int? AssignedToId { get; set; }
    
    public BlockerSeverity Severity { get; set; } = BlockerSeverity.Medium;
}

public class UpdateBlockerDto
{
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public int? AssignedToId { get; set; }
    
    public BlockerStatus? Status { get; set; }
    
    public BlockerSeverity? Severity { get; set; }
    
    [MaxLength(1000)]
    public string? Resolution { get; set; }
}

public class BlockerDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int? TaskId { get; set; }
    public string? TaskTitle { get; set; }
    public int ReportedById { get; set; }
    public string ReportedByName { get; set; } = string.Empty;
    public int? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public int SprintId { get; set; }
    public BlockerStatus Status { get; set; }
    public BlockerSeverity Severity { get; set; }
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? AiSuggestion { get; set; }
    public int DaysOpen { get; set; }
}
