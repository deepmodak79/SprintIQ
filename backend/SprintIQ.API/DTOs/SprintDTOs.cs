using System.ComponentModel.DataAnnotations;
using SprintIQ.API.Models;

namespace SprintIQ.API.DTOs;

public class CreateSprintDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Goal { get; set; }
    
    [Required]
    public int TeamId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
}

public class UpdateSprintDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(500)]
    public string? Goal { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public SprintStatus? Status { get; set; }
}

public class SprintDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SprintStatus Status { get; set; }
    public int TotalStoryPoints { get; set; }
    public int CompletedStoryPoints { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double ProgressPercentage { get; set; }
    public int DaysRemaining { get; set; }
    public List<SprintTaskDto> Tasks { get; set; } = new();
    public List<BurndownDataDto> BurndownData { get; set; } = new();
}

public class SprintSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public SprintStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double ProgressPercentage { get; set; }
    public int DaysRemaining { get; set; }
}

public class BurndownDataDto
{
    public DateTime Date { get; set; }
    public int RemainingPoints { get; set; }
    public int IdealRemainingPoints { get; set; }
    public int CompletedPoints { get; set; }
}
