using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.Models;

public class DailyStandup
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public int SprintId { get; set; }
    public virtual Sprint Sprint { get; set; } = null!;
    
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    
    [MaxLength(1000)]
    public string? Yesterday { get; set; }
    
    [MaxLength(1000)]
    public string? Today { get; set; }
    
    [MaxLength(1000)]
    public string? Blockers { get; set; }
    
    // Mood: 1-5 (1=Struggling, 3=Okay, 5=Great)
    public int? Mood { get; set; }
    
    // Confidence in sprint completion: 1-5
    public int? Confidence { get; set; }
    
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    // AI-generated summary for managers
    [MaxLength(2000)]
    public string? AiSummary { get; set; }
    
    public int PointsEarned { get; set; } = 10; // Base points for submitting standup
}
