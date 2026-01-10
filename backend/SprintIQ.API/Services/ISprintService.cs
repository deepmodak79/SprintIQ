using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface ISprintService
{
    Task<SprintDto?> CreateSprintAsync(CreateSprintDto dto);
    Task<SprintDto?> GetSprintByIdAsync(int sprintId);
    Task<List<SprintSummaryDto>> GetSprintsByTeamAsync(int teamId);
    Task<SprintDto?> UpdateSprintAsync(int sprintId, UpdateSprintDto dto);
    Task<bool> DeleteSprintAsync(int sprintId);
    Task<SprintDto?> GetActiveSprintByTeamAsync(int teamId);
    Task UpdateBurndownDataAsync(int sprintId);
    Task<List<BurndownDataDto>> GetBurndownDataAsync(int sprintId);
}
