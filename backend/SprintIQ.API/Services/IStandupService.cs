using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface IStandupService
{
    Task<StandupDto?> CreateOrUpdateStandupAsync(int userId, CreateStandupDto dto);
    Task<StandupDto?> GetStandupByIdAsync(int standupId);
    Task<StandupDto?> GetTodayStandupAsync(int userId, int sprintId);
    Task<List<StandupDto>> GetStandupsBySprintAsync(int sprintId, DateTime? date = null);
    Task<TeamStandupSummaryDto> GetTeamStandupSummaryAsync(int sprintId, DateTime date);
    Task<List<StandupDto>> GetUserStandupsAsync(int userId, int? limit = null);
}
