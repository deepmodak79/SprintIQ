using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetGlobalLeaderboard([FromQuery] int? limit = 10)
    {
        var leaderboard = await _leaderboardService.GetGlobalLeaderboardAsync(limit);
        return Ok(leaderboard);
    }

    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<TeamLeaderboardDto>> GetTeamLeaderboard(int teamId)
    {
        var leaderboard = await _leaderboardService.GetTeamLeaderboardAsync(teamId);
        if (leaderboard == null) return NotFound();
        return Ok(leaderboard);
    }

    [HttpGet("sprint/{sprintId}")]
    public async Task<ActionResult<SprintLeaderboardDto>> GetSprintLeaderboard(int sprintId)
    {
        var leaderboard = await _leaderboardService.GetSprintLeaderboardAsync(sprintId);
        if (leaderboard == null) return NotFound();
        return Ok(leaderboard);
    }

    [HttpGet("my-stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetMyStats()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var stats = await _leaderboardService.GetUserDashboardStatsAsync(userId.Value);
        return Ok(stats);
    }

    [HttpGet("team/{teamId}/dashboard")]
    public async Task<ActionResult<TeamDashboardDto>> GetTeamDashboard(int teamId)
    {
        var dashboard = await _leaderboardService.GetTeamDashboardAsync(teamId);
        if (dashboard == null) return NotFound();
        return Ok(dashboard);
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
