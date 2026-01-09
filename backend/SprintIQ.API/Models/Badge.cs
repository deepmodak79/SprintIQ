using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.Models;

public class Badge
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(50)]
    public string Icon { get; set; } = "🏆"; // Emoji or icon class
    
    [MaxLength(50)]
    public string Color { get; set; } = "#FFD700"; // Gold by default
    
    public BadgeType Type { get; set; }
    
    public int PointValue { get; set; } = 50;
    
    // Criteria for earning (e.g., "complete_tasks:10" means complete 10 tasks)
    [MaxLength(100)]
    public string? Criteria { get; set; }
    
    public int CriteriaValue { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
}

public enum BadgeType
{
    TaskCompletion,      // Complete X tasks
    StandupStreak,       // X consecutive standups
    BlockerBuster,       // Resolve X blockers
    SprintChampion,      // Complete sprint with 100% tasks
    EarlyBird,           // Submit standup before 9 AM
    TeamPlayer,          // Help resolve others' blockers
    VelocityKing,        // Highest velocity in sprint
    ConsistencyMaster    // Complete tasks on time consistently
}

public class UserBadge
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public int BadgeId { get; set; }
    public virtual Badge Badge { get; set; } = null!;
    
    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    
    // For badges that can be earned multiple times
    public int Count { get; set; } = 1;
}
