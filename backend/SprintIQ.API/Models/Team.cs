using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.Models;

public class Team
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
    public virtual ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
}

public class TeamMember
{
    public int Id { get; set; }
    
    public int TeamId { get; set; }
    public virtual Team Team { get; set; } = null!;
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    [MaxLength(50)]
    public string Role { get; set; } = "Member"; // Lead, Member
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
