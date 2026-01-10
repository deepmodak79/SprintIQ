using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface IAiService
{
    Task<string> GenerateStandupSummaryAsync(List<StandupDto> standups);
    Task<string> GenerateBlockerSuggestionAsync(string blockerDescription);
    Task<SprintHealthDto> AnalyzeSprintHealthAsync(int sprintId);
}
