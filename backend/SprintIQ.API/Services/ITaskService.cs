using SprintIQ.API.DTOs;
using TaskStatus = SprintIQ.API.Models.TaskStatus;

namespace SprintIQ.API.Services;

public interface ITaskService
{
    Task<SprintTaskDto?> CreateTaskAsync(CreateTaskDto dto);
    Task<SprintTaskDto?> GetTaskByIdAsync(int taskId);
    Task<List<SprintTaskDto>> GetTasksBySprintAsync(int sprintId);
    Task<KanbanBoardDto> GetKanbanBoardAsync(int sprintId);
    Task<SprintTaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto dto, int? userId = null);
    Task<SprintTaskDto?> MoveTaskAsync(int taskId, MoveTaskDto dto, int? userId = null);
    Task<bool> DeleteTaskAsync(int taskId);
}
