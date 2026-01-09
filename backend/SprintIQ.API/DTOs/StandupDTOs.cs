using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.DTOs;

public class CreateStandupDto
{
    [Required]
    public int SprintId { get; set; }
    
    [MaxLength(1000)]
    public string? Yesterday { get; set; }
    
    [MaxLength(1000)]
    public string? Today { get; set; }
    
    [MaxLength(1000)]
    public string? Blockers { get; set; }
    
    [Range(1, 5)]
    public int? Mood { get; set; }
    
    [Range(1, 5)]
    public int? Confidence { get; set; }
}

public class StandupDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public int SprintId { get; set; }
    public DateTime Date { get; set; }
    public string? Yesterday { get; set; }
    public string? Today { get; set; }
    public string? Blockers { get; set; }
    public int? Mood { get; set; }
    public int? Confidence { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? AiSummary { get; set; }
    public int PointsEarned { get; set; }
}

public class TeamStandupSummaryDto
{
    public DateTime Date { get; set; }
    public int TotalMembers { get; set; }
    public int SubmittedCount { get; set; }
    public double AverageMood { get; set; }
    public double AverageConfidence { get; set; }
    public List<StandupDto> Standups { get; set; } = new();
    public string? AiTeamSummary { get; set; }
    public List<string> CommonBlockers { get; set; } = new();
}

public class GenerateAiSummaryDto
{
    [Required]
    public int SprintId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
}
