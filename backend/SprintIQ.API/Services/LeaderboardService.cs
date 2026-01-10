using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;
using TaskStatus = SprintIQ.API.Models.TaskStatus;

namespace SprintIQ.API.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly SprintIQDbContext _context;

    public LeaderboardService(SprintIQDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeaderboardEntryDto>> GetGlobalLeaderboardAsync(int? limit = null)
    {
        var query = _context.Users
            .Include(u => u.Badges)
            .ThenInclude(ub => ub.Badge)
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.TotalPoints);

        if (limit.HasValue)
        {
            query = (IOrderedQueryable<User>)query.Take(limit.Value);
        }

        var users = await query.ToListAsync();
        var rank = 1;

        var leaderboard = new List<LeaderboardEntryDto>();

        foreach (var user in users)
        {
            var tasksCompleted = await _context.SprintTasks
                .CountAsync(t => t.AssigneeId == user.Id && t.Status == TaskStatus.Done);

            var storyPointsDelivered = await _context.SprintTasks
                .Where(t => t.AssigneeId == user.Id && t.Status == TaskStatus.Done)
                .SumAsync(t => t.StoryPoints);

            var blockersResolved = await _context.Blockers
                .CountAsync(b => b.AssignedToId == user.Id && b.Status == BlockerStatus.Resolved);

            var topBadge = user.Badges
                .OrderByDescending(ub => ub.Badge.PointValue)
                .FirstOrDefault()?.Badge;

            leaderboard.Add(new LeaderboardEntryDto
            {
                Rank = rank++,
                UserId = user.Id,
                UserName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                TotalPoints = user.TotalPoints,
                TasksCompleted = tasksCompleted,
                StoryPointsDelivered = storyPointsDelivered,
                CurrentStreak = user.CurrentStreak,
                BlockersResolved = blockersResolved,
                BadgeCount = user.Badges.Count,
                TopBadgeIcon = topBadge?.Icon ?? "🏆"
            });
        }

        return leaderboard;
    }

    public async Task<TeamLeaderboardDto?> GetTeamLeaderboardAsync(int teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .ThenInclude(tm => tm.User)
            .ThenInclude(u => u.Badges)
            .ThenInclude(ub => ub.Badge)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null) return null;

        var members = team.Members.Select(m => m.User).ToList();
        var rank = 1;

        var memberEntries = new List<LeaderboardEntryDto>();

        foreach (var user in members.OrderByDescending(u => u.TotalPoints))
        {
            var tasksCompleted = await _context.SprintTasks
                .CountAsync(t => t.AssigneeId == user.Id && t.Status == TaskStatus.Done);

            var storyPointsDelivered = await _context.SprintTasks
                .Where(t => t.AssigneeId == user.Id && t.Status == TaskStatus.Done)
                .SumAsync(t => t.StoryPoints);

            var blockersResolved = await _context.Blockers
                .CountAsync(b => b.AssignedToId == user.Id && b.Status == BlockerStatus.Resolved);

            var topBadge = user.Badges
                .OrderByDescending(ub => ub.Badge.PointValue)
                .FirstOrDefault()?.Badge;

            memberEntries.Add(new LeaderboardEntryDto
            {
                Rank = rank++,
                UserId = user.Id,
                UserName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                TotalPoints = user.TotalPoints,
                TasksCompleted = tasksCompleted,
                StoryPointsDelivered = storyPointsDelivered,
                CurrentStreak = user.CurrentStreak,
                BlockersResolved = blockersResolved,
                BadgeCount = user.Badges.Count,
                TopBadgeIcon = topBadge?.Icon ?? "🏆"
            });
        }

        return new TeamLeaderboardDto
        {
            TeamId = team.Id,
            TeamName = team.Name,
            Members = memberEntries,
            TotalTeamPoints = memberEntries.Sum(m => m.TotalPoints),
            AveragePoints = memberEntries.Any() ? memberEntries.Average(m => m.TotalPoints) : 0
        };
    }

    public async Task<SprintLeaderboardDto?> GetSprintLeaderboardAsync(int sprintId)
    {
        var sprint = await _context.Sprints
            .Include(s => s.Tasks)
            .ThenInclude(t => t.Assignee)
            .FirstOrDefaultAsync(s => s.Id == sprintId);

        if (sprint == null) return null;

        var completedTasks = sprint.Tasks.Where(t => t.Status == TaskStatus.Done && t.AssigneeId.HasValue);
        var userPoints = completedTasks
            .GroupBy(t => t.AssigneeId!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                Points = g.Sum(t => t.PointsAwarded),
                TasksCompleted = g.Count(),
                StoryPoints = g.Sum(t => t.StoryPoints)
            })
            .OrderByDescending(x => x.Points)
            .ToList();

        var topPerformers = new List<LeaderboardEntryDto>();
        var rank = 1;

        foreach (var up in userPoints.Take(10))
        {
            var user = await _context.Users
                .Include(u => u.Badges)
                .ThenInclude(ub => ub.Badge)
                .FirstOrDefaultAsync(u => u.Id == up.UserId);

            if (user == null) continue;

            var topBadge = user.Badges
                .OrderByDescending(ub => ub.Badge.PointValue)
                .FirstOrDefault()?.Badge;

            topPerformers.Add(new LeaderboardEntryDto
            {
                Rank = rank++,
                UserId = user.Id,
                UserName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                TotalPoints = up.Points,
                TasksCompleted = up.TasksCompleted,
                StoryPointsDelivered = up.StoryPoints,
                CurrentStreak = user.CurrentStreak,
                BadgeCount = user.Badges.Count,
                TopBadgeIcon = topBadge?.Icon ?? "🏆"
            });
        }

        return new SprintLeaderboardDto
        {
            SprintId = sprint.Id,
            SprintName = sprint.Name,
            TopPerformers = topPerformers,
            MvpOfSprint = topPerformers.FirstOrDefault()
        };
    }

    public async Task<DashboardStatsDto> GetUserDashboardStatsAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Badges)
            .ThenInclude(ub => ub.Badge)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return new DashboardStatsDto();

        var tasksCompleted = await _context.SprintTasks
            .CountAsync(t => t.AssigneeId == userId && t.Status == TaskStatus.Done);

        var totalStoryPoints = await _context.SprintTasks
            .Where(t => t.AssigneeId == userId && t.Status == TaskStatus.Done)
            .SumAsync(t => t.StoryPoints);

        var blockersResolved = await _context.Blockers
            .CountAsync(b => b.AssignedToId == userId && b.Status == BlockerStatus.Resolved);

        var totalUsers = await _context.Users.CountAsync(u => u.IsActive);
        var rank = await _context.Users
            .CountAsync(u => u.IsActive && u.TotalPoints > user.TotalPoints) + 1;

        var recentBadges = user.Badges
            .OrderByDescending(ub => ub.EarnedAt)
            .Take(5)
            .Select(ub => new BadgeDto
            {
                Id = ub.Badge.Id,
                Name = ub.Badge.Name,
                Description = ub.Badge.Description,
                Icon = ub.Badge.Icon,
                Color = ub.Badge.Color,
                EarnedAt = ub.EarnedAt,
                Count = ub.Count
            })
            .ToList();

        return new DashboardStatsDto
        {
            TotalTasksCompleted = tasksCompleted,
            TotalStoryPoints = totalStoryPoints,
            CurrentStreak = user.CurrentStreak,
            TotalPoints = user.TotalPoints,
            Rank = rank,
            TotalUsers = totalUsers,
            BadgesEarned = user.Badges.Count,
            BlockersResolved = blockersResolved,
            RecentBadges = recentBadges
        };
    }

    public async Task<TeamDashboardDto?> GetTeamDashboardAsync(int teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null) return null;

        var activeSprint = await _context.Sprints
            .Include(s => s.Tasks)
            .Where(s => s.TeamId == teamId && s.Status == SprintStatus.Active)
            .FirstOrDefaultAsync();

        if (activeSprint == null)
        {
            return new TeamDashboardDto
            {
                TeamMemberCount = team.Members.Count
            };
        }

        var today = DateTime.UtcNow.Date;
        var todayStandups = await _context.DailyStandups
            .Where(s => s.SprintId == activeSprint.Id && s.Date == today)
            .ToListAsync();

        var openBlockers = await _context.Blockers
            .CountAsync(b => b.SprintId == activeSprint.Id && b.Status != BlockerStatus.Resolved);

        var totalTasks = activeSprint.Tasks.Count;
        var completedTasks = activeSprint.Tasks.Count(t => t.Status == TaskStatus.Done);
        var progress = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;
        var daysRemaining = Math.Max(0, (activeSprint.EndDate - DateTime.UtcNow).Days);

        // Calculate sprint health
        var sprintHealth = CalculateSprintHealth(activeSprint, progress, openBlockers, todayStandups.Count, team.Members.Count);

        return new TeamDashboardDto
        {
            ActiveSprintId = activeSprint.Id,
            ActiveSprintName = activeSprint.Name,
            SprintProgress = Math.Round(progress, 1),
            DaysRemaining = daysRemaining,
            TeamMemberCount = team.Members.Count,
            TodayStandupCount = todayStandups.Count,
            OpenBlockers = openBlockers,
            AverageTeamMood = todayStandups.Where(s => s.Mood.HasValue).DefaultIfEmpty().Average(s => s?.Mood ?? 3),
            AverageTeamConfidence = todayStandups.Where(s => s.Confidence.HasValue).DefaultIfEmpty().Average(s => s?.Confidence ?? 3),
            SprintHealth = sprintHealth
        };
    }

    public async Task AddPointsAsync(int userId, int points, string activityType, string description)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        user.TotalPoints += points;
        user.LastActiveAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task CheckAndAwardBadgesAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Badges)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return;

        var badges = await _context.Badges.Where(b => b.IsActive).ToListAsync();

        var tasksCompleted = await _context.SprintTasks
            .CountAsync(t => t.AssigneeId == userId && t.Status == TaskStatus.Done);

        var standupCount = await _context.DailyStandups.CountAsync(s => s.UserId == userId);

        var blockersResolved = await _context.Blockers
            .CountAsync(b => b.AssignedToId == userId && b.Status == BlockerStatus.Resolved);

        foreach (var badge in badges)
        {
            var alreadyHas = user.Badges.Any(ub => ub.BadgeId == badge.Id);
            if (alreadyHas) continue;

            var earned = badge.Type switch
            {
                BadgeType.TaskCompletion => tasksCompleted >= badge.CriteriaValue,
                BadgeType.StandupStreak => user.CurrentStreak >= badge.CriteriaValue,
                BadgeType.BlockerBuster => blockersResolved >= badge.CriteriaValue,
                _ => false
            };

            if (earned)
            {
                _context.UserBadges.Add(new UserBadge
                {
                    UserId = userId,
                    BadgeId = badge.Id,
                    EarnedAt = DateTime.UtcNow
                });

                user.TotalPoints += badge.PointValue;
            }
        }

        await _context.SaveChangesAsync();
    }

    private static SprintHealthDto CalculateSprintHealth(Sprint sprint, double progress, int openBlockers, int todayStandups, int teamSize)
    {
        var warnings = new List<string>();
        var recommendations = new List<string>();

        var totalDays = (sprint.EndDate - sprint.StartDate).Days;
        var daysElapsed = (DateTime.UtcNow - sprint.StartDate).Days;
        var expectedProgress = totalDays > 0 ? (double)daysElapsed / totalDays * 100 : 0;

        // Check progress vs expected
        if (progress < expectedProgress - 20)
        {
            warnings.Add("Sprint is behind schedule");
            recommendations.Add("Consider reducing scope or adding resources");
        }

        // Check blockers
        if (openBlockers > 3)
        {
            warnings.Add($"{openBlockers} unresolved blockers");
            recommendations.Add("Prioritize blocker resolution in today's standup");
        }

        // Check standup participation
        var standupRate = teamSize > 0 ? (double)todayStandups / teamSize * 100 : 0;
        if (standupRate < 50)
        {
            warnings.Add("Low standup participation today");
            recommendations.Add("Remind team members to submit their daily updates");
        }

        var status = "Healthy";
        var color = "#4CAF50";

        if (warnings.Count >= 3)
        {
            status = "Critical";
            color = "#F44336";
        }
        else if (warnings.Count >= 1)
        {
            status = "AtRisk";
            color = "#FF9800";
        }

        return new SprintHealthDto
        {
            Status = status,
            Color = color,
            Warnings = warnings,
            Recommendations = recommendations
        };
    }
}
