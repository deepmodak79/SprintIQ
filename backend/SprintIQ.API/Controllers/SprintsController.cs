using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SprintsController : ControllerBase
{
    private readonly ISprintService _sprintService;

    public SprintsController(ISprintService sprintService)
    {
        _sprintService = sprintService;
    }

    [HttpPost]
    public async Task<ActionResult<SprintDto>> CreateSprint([FromBody] CreateSprintDto dto)
    {
        var sprint = await _sprintService.CreateSprintAsync(dto);
        if (sprint == null) return BadRequest(new { message = "Invalid team ID" });
        return CreatedAtAction(nameof(GetSprint), new { id = sprint.Id }, sprint);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SprintDto>> GetSprint(int id)
    {
        var sprint = await _sprintService.GetSprintByIdAsync(id);
        if (sprint == null) return NotFound();
        return Ok(sprint);
    }

    [HttpGet("team/{teamId}")]
    public async Task<ActionResult<List<SprintSummaryDto>>> GetSprintsByTeam(int teamId)
    {
        var sprints = await _sprintService.GetSprintsByTeamAsync(teamId);
        return Ok(sprints);
    }

    [HttpGet("team/{teamId}/active")]
    public async Task<ActionResult<SprintDto>> GetActiveSprintByTeam(int teamId)
    {
        var sprint = await _sprintService.GetActiveSprintByTeamAsync(teamId);
        if (sprint == null) return NotFound(new { message = "No active sprint found" });
        return Ok(sprint);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SprintDto>> UpdateSprint(int id, [FromBody] UpdateSprintDto dto)
    {
        var sprint = await _sprintService.UpdateSprintAsync(id, dto);
        if (sprint == null) return NotFound();
        return Ok(sprint);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSprint(int id)
    {
        var result = await _sprintService.DeleteSprintAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpGet("{id}/burndown")]
    public async Task<ActionResult<List<BurndownDataDto>>> GetBurndownData(int id)
    {
        var data = await _sprintService.GetBurndownDataAsync(id);
        return Ok(data);
    }

    [HttpPost("{id}/burndown/update")]
    public async Task<ActionResult> UpdateBurndownData(int id)
    {
        await _sprintService.UpdateBurndownDataAsync(id);
        return Ok();
    }
}
