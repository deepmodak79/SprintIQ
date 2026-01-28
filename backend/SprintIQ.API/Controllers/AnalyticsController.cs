using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAiService aiService, ILogger<AnalyticsController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Analyze sprint risk and get AI predictions
    /// </summary>
    [HttpGet("sprint/{sprintId}/risk")]
    public async Task<ActionResult<SprintRiskResponse>> GetSprintRisk(int sprintId)
    {
        try
        {
            var risk = await _aiService.AnalyzeSprintRiskAsync(sprintId);
            return Ok(risk);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze sprint risk for sprint {SprintId}", sprintId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Predict potential blockers for sprint tasks
    /// </summary>
    [HttpGet("sprint/{sprintId}/blocker-predictions")]
    public async Task<ActionResult<List<BlockerPredictionResponse>>> GetBlockerPredictions(int sprintId)
    {
        try
        {
            var predictions = await _aiService.PredictBlockersAsync(sprintId);
            return Ok(predictions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to predict blockers for sprint {SprintId}", sprintId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get team health metrics and insights
    /// </summary>
    [HttpGet("team/{teamId}/health")]
    public async Task<ActionResult<TeamHealthResponse>> GetTeamHealth(int teamId)
    {
        try
        {
            var health = await _aiService.AnalyzeTeamHealthAsync(teamId);
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze team health for team {TeamId}", teamId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Generate smart standup with AI insights
    /// </summary>
    [HttpGet("team/{teamId}/smart-standup")]
    public async Task<ActionResult<SmartStandupResponse>> GetSmartStandup(int teamId, [FromQuery] DateTime? date = null)
    {
        try
        {
            var standupDate = date ?? DateTime.UtcNow;
            var standup = await _aiService.GenerateSmartStandupAsync(teamId, standupDate);
            return Ok(standup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate smart standup for team {TeamId}", teamId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get AI-generated insights for a sprint
    /// </summary>
    [HttpGet("sprint/{sprintId}/insights")]
    public async Task<ActionResult<List<string>>> GetSprintInsights(int sprintId)
    {
        try
        {
            var insights = await _aiService.GenerateAIInsightsAsync(sprintId);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate insights for sprint {SprintId}", sprintId);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get sprint health analysis (legacy endpoint)
    /// </summary>
    [HttpGet("sprint/{sprintId}/health")]
    public async Task<ActionResult<SprintHealthDto>> GetSprintHealth(int sprintId)
    {
        try
        {
            var health = await _aiService.AnalyzeSprintHealthAsync(sprintId);
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze sprint health for sprint {SprintId}", sprintId);
            return BadRequest(new { error = ex.Message });
        }
    }
}
