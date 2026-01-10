using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface ILeaderboardService
{
    Task<List<LeaderboardEntryDto>> GetGlobalLeaderboardAsync(int? limit = null);
    Task<TeamLeaderboardDto?> GetTeamLeaderboardAsync(int teamId);
    Task<SprintLeaderboardDto?> GetSprintLeaderboardAsync(int sprintId);
    Task<DashboardStatsDto> GetUserDashboardStatsAsync(int userId);
    Task<TeamDashboardDto?> GetTeamDashboardAsync(int teamId);
    Task AddPointsAsync(int userId, int points, string activityType, string description);
    Task CheckAndAwardBadgesAsync(int userId);
}
