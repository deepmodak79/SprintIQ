using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlockersController : ControllerBase
{
    private readonly IBlockerService _blockerService;
    private readonly IAiService _aiService;

    public BlockersController(IBlockerService blockerService, IAiService aiService)
    {
        _blockerService = blockerService;
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<ActionResult<BlockerDto>> CreateBlocker([FromBody] CreateBlockerDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var blocker = await _blockerService.CreateBlockerAsync(userId.Value, dto);
        if (blocker == null) return BadRequest(new { message = "Invalid sprint ID" });
        return CreatedAtAction(nameof(GetBlocker), new { id = blocker.Id }, blocker);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BlockerDto>> GetBlocker(int id)
    {
        var blocker = await _blockerService.GetBlockerByIdAsync(id);
        if (blocker == null) return NotFound();
        return Ok(blocker);
    }

    [HttpGet("sprint/{sprintId}")]
    public async Task<ActionResult<List<BlockerDto>>> GetBlockersBySprint(int sprintId)
    {
        var blockers = await _blockerService.GetBlockersBySprintAsync(sprintId);
        return Ok(blockers);
    }

    [HttpGet("open")]
    public async Task<ActionResult<List<BlockerDto>>> GetOpenBlockers([FromQuery] int? sprintId = null)
    {
        var blockers = await _blockerService.GetOpenBlockersAsync(sprintId);
        return Ok(blockers);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BlockerDto>> UpdateBlocker(int id, [FromBody] UpdateBlockerDto dto)
    {
        var userId = GetCurrentUserId();
        var blocker = await _blockerService.UpdateBlockerAsync(id, dto, userId);
        if (blocker == null) return NotFound();
        return Ok(blocker);
    }

    [HttpPost("{id}/resolve")]
    public async Task<ActionResult<BlockerDto>> ResolveBlocker(int id, [FromBody] ResolveBlockerDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var blocker = await _blockerService.ResolveBlockerAsync(id, dto.Resolution, userId.Value);
        if (blocker == null) return NotFound();
        return Ok(blocker);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBlocker(int id)
    {
        var result = await _blockerService.DeleteBlockerAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("suggest")]
    public async Task<ActionResult<string>> GetAiSuggestion([FromBody] BlockerSuggestionDto dto)
    {
        var suggestion = await _aiService.GenerateBlockerSuggestionAsync(dto.Description);
        return Ok(new { suggestion });
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

public class ResolveBlockerDto
{
    public string Resolution { get; set; } = string.Empty;
}

public class BlockerSuggestionDto
{
    public string Description { get; set; } = string.Empty;
}
