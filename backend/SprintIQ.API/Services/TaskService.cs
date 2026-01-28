using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;
using TaskStatus = SprintIQ.API.Models.TaskStatus;

namespace SprintIQ.API.Services;

public class TaskService : ITaskService
{
    private readonly SprintIQDbContext _context;
    private readonly ISprintService _sprintService;
    private readonly ILeaderboardService _leaderboardService;

    public TaskService(SprintIQDbContext context, ISprintService sprintService, ILeaderboardService leaderboardService)
    {
        _context = context;
        _sprintService = sprintService;
        _leaderboardService = leaderboardService;
    }

    public async Task<SprintTaskDto?> CreateTaskAsync(CreateTaskDto dto)
    {
        var sprint = await _context.Sprints.FindAsync(dto.SprintId);
        if (sprint == null) return null;

        var maxOrder = await _context.Tasks
            .Where(t => t.SprintId == dto.SprintId)
            .MaxAsync(t => (int?)t.OrderIndex) ?? 0;

        var task = new SprintTask
        {
            Title = dto.Title,
            Description = dto.Description,
            SprintId = dto.SprintId,
            AssigneeId = dto.AssigneeId,
            Priority = dto.Priority,
            StoryPoints = dto.StoryPoints,
            OrderIndex = maxOrder + 1,
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        await _sprintService.UpdateBurndownDataAsync(dto.SprintId);

        return await GetTaskByIdAsync(task.Id);
    }

    public async Task<SprintTaskDto?> GetTaskByIdAsync(int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Blockers)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        return task == null ? null : MapToTaskDto(task);
    }

    public async Task<List<SprintTaskDto>> GetTasksBySprintAsync(int sprintId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Blockers)
            .Where(t => t.SprintId == sprintId)
            .OrderBy(t => t.OrderIndex)
            .ToListAsync();

        return tasks.Select(MapToTaskDto).ToList();
    }

    public async Task<KanbanBoardDto> GetKanbanBoardAsync(int sprintId)
    {
        var tasks = await GetTasksBySprintAsync(sprintId);

        return new KanbanBoardDto
        {
            Todo = tasks.Where(t => t.Status == TaskStatus.Todo).OrderBy(t => t.OrderIndex).ToList(),
            InProgress = tasks.Where(t => t.Status == TaskStatus.InProgress).OrderBy(t => t.OrderIndex).ToList(),
            InReview = tasks.Where(t => t.Status == TaskStatus.InReview).OrderBy(t => t.OrderIndex).ToList(),
            Done = tasks.Where(t => t.Status == TaskStatus.Done).OrderBy(t => t.OrderIndex).ToList()
        };
    }

    public async Task<SprintTaskDto?> UpdateTaskAsync(int taskId, UpdateTaskDto dto, int? userId = null)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return null;

        var previousStatus = task.Status;

        if (dto.Title != null) task.Title = dto.Title;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.AssigneeId.HasValue) task.AssigneeId = dto.AssigneeId;
        if (dto.Status.HasValue) task.Status = dto.Status.Value;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        if (dto.StoryPoints.HasValue) task.StoryPoints = dto.StoryPoints.Value;
        if (dto.OrderIndex.HasValue) task.OrderIndex = dto.OrderIndex.Value;
        if (dto.IsBlocked.HasValue) task.IsBlocked = dto.IsBlocked.Value;
        if (dto.BlockedReason != null) task.BlockedReason = dto.BlockedReason;

        // Track status changes
        if (dto.Status.HasValue && dto.Status.Value != previousStatus)
        {
            if (dto.Status.Value == TaskStatus.InProgress && task.StartedAt == null)
            {
                task.StartedAt = DateTime.UtcNow;
            }
            else if (dto.Status.Value == TaskStatus.Done && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.UtcNow;
                
                // Award points
                var points = CalculateTaskPoints(task);
                task.PointsAwarded = points;

                if (task.AssigneeId.HasValue)
                {
                    await _leaderboardService.AddPointsAsync(task.AssigneeId.Value, points, "TaskCompleted", $"Completed task: {task.Title}");
                    await _leaderboardService.CheckAndAwardBadgesAsync(task.AssigneeId.Value);
                }
            }
        }

        await _context.SaveChangesAsync();
        await _sprintService.UpdateBurndownDataAsync(task.SprintId);

        return await GetTaskByIdAsync(taskId);
    }

    public async Task<SprintTaskDto?> MoveTaskAsync(int taskId, MoveTaskDto dto, int? userId = null)
    {
        var updateDto = new UpdateTaskDto
        {
            Status = dto.NewStatus,
            OrderIndex = dto.NewOrderIndex
        };

        return await UpdateTaskAsync(taskId, updateDto, userId);
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        var sprintId = task.SprintId;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        await _sprintService.UpdateBurndownDataAsync(sprintId);

        return true;
    }

    private static int CalculateTaskPoints(SprintTask task)
    {
        var basePoints = 20;
        var storyPointBonus = task.StoryPoints * 10;
        var priorityBonus = task.Priority switch
        {
            TaskPriority.Critical => 30,
            TaskPriority.High => 20,
            TaskPriority.Medium => 10,
            _ => 5
        };

        return basePoints + storyPointBonus + priorityBonus;
    }

    private static SprintTaskDto MapToTaskDto(SprintTask task)
    {
        return new SprintTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            SprintId = task.SprintId,
            AssigneeId = task.AssigneeId,
            AssigneeName = task.Assignee?.FullName,
            AssigneeAvatar = task.Assignee?.AvatarUrl,
            Status = task.Status,
            Priority = task.Priority,
            StoryPoints = task.StoryPoints,
            OrderIndex = task.OrderIndex,
            IsBlocked = task.IsBlocked,
            BlockedReason = task.BlockedReason,
            CreatedAt = task.CreatedAt,
            StartedAt = task.StartedAt,
            CompletedAt = task.CompletedAt,
            BlockerCount = task.Blockers?.Count(b => b.Status != BlockerStatus.Resolved) ?? 0
        };
    }
}
