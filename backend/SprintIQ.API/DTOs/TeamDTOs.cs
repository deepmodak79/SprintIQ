using System.ComponentModel.DataAnnotations;

namespace SprintIQ.API.DTOs;

public class CreateTeamDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateTeamDto
{
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
}

public class AddTeamMemberDto
{
    [Required]
    public int UserId { get; set; }
    
    [MaxLength(50)]
    public string Role { get; set; } = "Member";
}

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
    public int ActiveSprintCount { get; set; }
    public List<TeamMemberDto> Members { get; set; } = new();
}

public class TeamMemberDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public int TotalPoints { get; set; }
}
