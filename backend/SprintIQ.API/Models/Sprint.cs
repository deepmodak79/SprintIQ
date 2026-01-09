using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.Models;

public class Sprint
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Goal { get; set; }
    
    public int TeamId { get; set; }
    public virtual Team Team { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public SprintStatus Status { get; set; } = SprintStatus.Planning;
    
    public int TotalStoryPoints { get; set; } = 0;
    
    public int CompletedStoryPoints { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<SprintTask> Tasks { get; set; } = new List<SprintTask>();
    public virtual ICollection<DailyStandup> Standups { get; set; } = new List<DailyStandup>();
    public virtual ICollection<SprintBurndown> BurndownData { get; set; } = new List<SprintBurndown>();
}

public enum SprintStatus
{
    Planning,
    Active,
    Completed,
    Cancelled
}

public class SprintBurndown
{
    public int Id { get; set; }
    
    public int SprintId { get; set; }
    public virtual Sprint Sprint { get; set; } = null!;
    
    public DateTime Date { get; set; }
    
    public int RemainingPoints { get; set; }
    
    public int IdealRemainingPoints { get; set; }
    
    public int CompletedPoints { get; set; }
}
