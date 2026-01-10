using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<ActionResult<SprintTaskDto>> CreateTask([FromBody] CreateTaskDto dto)
    {
        var task = await _taskService.CreateTaskAsync(dto);
        if (task == null) return BadRequest(new { message = "Invalid sprint ID" });
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SprintTaskDto>> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpGet("sprint/{sprintId}")]
    public async Task<ActionResult<List<SprintTaskDto>>> GetTasksBySprint(int sprintId)
    {
        var tasks = await _taskService.GetTasksBySprintAsync(sprintId);
        return Ok(tasks);
    }

    [HttpGet("sprint/{sprintId}/kanban")]
    public async Task<ActionResult<KanbanBoardDto>> GetKanbanBoard(int sprintId)
    {
        var board = await _taskService.GetKanbanBoardAsync(sprintId);
        return Ok(board);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SprintTaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        var userId = GetCurrentUserId();
        var task = await _taskService.UpdateTaskAsync(id, dto, userId);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPut("{id}/move")]
    public async Task<ActionResult<SprintTaskDto>> MoveTask(int id, [FromBody] MoveTaskDto dto)
    {
        var userId = GetCurrentUserId();
        var task = await _taskService.MoveTaskAsync(id, dto, userId);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);
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
