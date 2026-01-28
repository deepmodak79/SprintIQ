using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface IAiService
{
    Task<string> GenerateStandupSummaryAsync(List<StandupDto> standups);
    Task<string> GenerateBlockerSuggestionAsync(string blockerDescription);
    Task<SprintHealthDto> AnalyzeSprintHealthAsync(int sprintId);
    
    // SprintIQ 2.0 - Advanced AI Features
    Task<SprintRiskResponse> AnalyzeSprintRiskAsync(int sprintId);
    Task<List<BlockerPredictionResponse>> PredictBlockersAsync(int sprintId);
    Task<TeamHealthResponse> AnalyzeTeamHealthAsync(int teamId);
    Task<SmartStandupResponse> GenerateSmartStandupAsync(int teamId, DateTime date);
    Task<List<string>> GenerateAIInsightsAsync(int sprintId);
}
