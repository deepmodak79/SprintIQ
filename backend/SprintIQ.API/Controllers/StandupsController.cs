using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StandupsController : ControllerBase
{
    private readonly IStandupService _standupService;
    private readonly IAiService _aiService;

    public StandupsController(IStandupService standupService, IAiService aiService)
    {
        _standupService = standupService;
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<ActionResult<StandupDto>> CreateOrUpdateStandup([FromBody] CreateStandupDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var standup = await _standupService.CreateOrUpdateStandupAsync(userId.Value, dto);
        if (standup == null) return BadRequest(new { message = "Invalid sprint ID" });
        return Ok(standup);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StandupDto>> GetStandup(int id)
    {
        var standup = await _standupService.GetStandupByIdAsync(id);
        if (standup == null) return NotFound();
        return Ok(standup);
    }

    [HttpGet("today")]
    public async Task<ActionResult<StandupDto>> GetTodayStandup([FromQuery] int sprintId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var standup = await _standupService.GetTodayStandupAsync(userId.Value, sprintId);
        if (standup == null) return NotFound();
        return Ok(standup);
    }

    [HttpGet("sprint/{sprintId}")]
    public async Task<ActionResult<List<StandupDto>>> GetStandupsBySprint(int sprintId, [FromQuery] DateTime? date = null)
    {
        var standups = await _standupService.GetStandupsBySprintAsync(sprintId, date);
        return Ok(standups);
    }

    [HttpGet("sprint/{sprintId}/summary")]
    public async Task<ActionResult<TeamStandupSummaryDto>> GetTeamStandupSummary(int sprintId, [FromQuery] DateTime? date = null)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;
        var summary = await _standupService.GetTeamStandupSummaryAsync(sprintId, targetDate);
        return Ok(summary);
    }

    [HttpGet("my-standups")]
    public async Task<ActionResult<List<StandupDto>>> GetMyStandups([FromQuery] int? limit = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var standups = await _standupService.GetUserStandupsAsync(userId.Value, limit);
        return Ok(standups);
    }

    [HttpPost("generate-ai-summary")]
    public async Task<ActionResult<string>> GenerateAiSummary([FromBody] GenerateAiSummaryDto dto)
    {
        var standups = await _standupService.GetStandupsBySprintAsync(dto.SprintId, dto.Date);
        var summary = await _aiService.GenerateStandupSummaryAsync(standups);
        return Ok(new { summary });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }
        return userId;
    }
}
