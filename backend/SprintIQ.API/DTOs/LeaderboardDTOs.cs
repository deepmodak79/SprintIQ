namespace SprintIQ.API.DTOs;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int TotalPoints { get; set; }
    public int TasksCompleted { get; set; }
    public int StoryPointsDelivered { get; set; }
    public int CurrentStreak { get; set; }
    public int BlockersResolved { get; set; }
    public int BadgeCount { get; set; }
    public string TopBadgeIcon { get; set; } = "🏆";
}

public class TeamLeaderboardDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public List<LeaderboardEntryDto> Members { get; set; } = new();
    public int TotalTeamPoints { get; set; }
    public double AveragePoints { get; set; }
}

public class SprintLeaderboardDto
{
    public int SprintId { get; set; }
    public string SprintName { get; set; } = string.Empty;
    public List<LeaderboardEntryDto> TopPerformers { get; set; } = new();
    public LeaderboardEntryDto? MvpOfSprint { get; set; }
}

public class DashboardStatsDto
{
    public int TotalTasksCompleted { get; set; }
    public int TotalStoryPoints { get; set; }
    public int CurrentStreak { get; set; }
    public int TotalPoints { get; set; }
    public int Rank { get; set; }
    public int TotalUsers { get; set; }
    public int BadgesEarned { get; set; }
    public int BlockersResolved { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<BadgeDto> RecentBadges { get; set; } = new();
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // TaskCompleted, StandupSubmitted, BadgeEarned, BlockerResolved
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime Timestamp { get; set; }
}

public class TeamDashboardDto
{
    public int ActiveSprintId { get; set; }
    public string ActiveSprintName { get; set; } = string.Empty;
    public double SprintProgress { get; set; }
    public int DaysRemaining { get; set; }
    public int TeamMemberCount { get; set; }
    public int TodayStandupCount { get; set; }
    public int OpenBlockers { get; set; }
    public double TeamVelocity { get; set; }
    public double AverageTeamMood { get; set; }
    public double AverageTeamConfidence { get; set; }
    public SprintHealthDto SprintHealth { get; set; } = new();
}

public class SprintHealthDto
{
    public string Status { get; set; } = "Healthy"; // Healthy, AtRisk, Critical
    public string Color { get; set; } = "#4CAF50";
    public List<string> Warnings { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}
