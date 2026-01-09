using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? Role { get; set; } = "Member"; // Admin, ScrumMaster, Member
    
    [MaxLength(255)]
    public string? AvatarUrl { get; set; }
    
    public int TotalPoints { get; set; } = 0;
    
    public int CurrentStreak { get; set; } = 0;
    
    public int LongestStreak { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastActiveAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
    public virtual ICollection<SprintTask> AssignedTasks { get; set; } = new List<SprintTask>();
    public virtual ICollection<DailyStandup> Standups { get; set; } = new List<DailyStandup>();
    public virtual ICollection<UserBadge> Badges { get; set; } = new List<UserBadge>();
}
