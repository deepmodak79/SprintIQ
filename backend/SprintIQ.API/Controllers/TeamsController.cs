using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] CreateTeamDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var team = await _teamService.CreateTeamAsync(dto, userId.Value);
        return CreatedAtAction(nameof(GetTeam), new { id = team!.Id }, team);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetTeam(int id)
    {
        var team = await _teamService.GetTeamByIdAsync(id);
        if (team == null) return NotFound();
        return Ok(team);
    }

    [HttpGet]
    public async Task<ActionResult<List<TeamDto>>> GetAllTeams()
    {
        var teams = await _teamService.GetAllTeamsAsync();
        return Ok(teams);
    }

    [HttpGet("my-teams")]
    public async Task<ActionResult<List<TeamDto>>> GetMyTeams()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var teams = await _teamService.GetUserTeamsAsync(userId.Value);
        return Ok(teams);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TeamDto>> UpdateTeam(int id, [FromBody] UpdateTeamDto dto)
    {
        var team = await _teamService.UpdateTeamAsync(id, dto);
        if (team == null) return NotFound();
        return Ok(team);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTeam(int id)
    {
        var result = await _teamService.DeleteTeamAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/members")]
    public async Task<ActionResult<TeamDto>> AddMember(int id, [FromBody] AddTeamMemberDto dto)
    {
        var team = await _teamService.AddMemberAsync(id, dto);
        if (team == null) return NotFound();
        return Ok(team);
    }

    [HttpDelete("{teamId}/members/{userId}")]
    public async Task<ActionResult> RemoveMember(int teamId, int userId)
    {
        var result = await _teamService.RemoveMemberAsync(teamId, userId);
        if (!result) return NotFound();
        return NoContent();
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
