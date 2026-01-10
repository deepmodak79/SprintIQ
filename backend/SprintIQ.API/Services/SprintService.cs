using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;
using TaskStatus = SprintIQ.API.Models.TaskStatus;

namespace SprintIQ.API.Services;

public class SprintService : ISprintService
{
    private readonly SprintIQDbContext _context;

    public SprintService(SprintIQDbContext context)
    {
        _context = context;
    }

    public async Task<SprintDto?> CreateSprintAsync(CreateSprintDto dto)
    {
        var team = await _context.Teams.FindAsync(dto.TeamId);
        if (team == null) return null;

        var sprint = new Sprint
        {
            Name = dto.Name,
            Goal = dto.Goal,
            TeamId = dto.TeamId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = SprintStatus.Planning,
            CreatedAt = DateTime.UtcNow
        };

        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();

        // Initialize burndown data
        await InitializeBurndownDataAsync(sprint);

        return await GetSprintByIdAsync(sprint.Id);
    }

    public async Task<SprintDto?> GetSprintByIdAsync(int sprintId)
    {
        var sprint = await _context.Sprints
            .Include(s => s.Team)
            .Include(s => s.Tasks)
            .ThenInclude(t => t.Assignee)
            .Include(s => s.Tasks)
            .ThenInclude(t => t.Blockers)
            .Include(s => s.BurndownData)
            .FirstOrDefaultAsync(s => s.Id == sprintId);

        if (sprint == null) return null;

        return MapToSprintDto(sprint);
    }

    public async Task<List<SprintSummaryDto>> GetSprintsByTeamAsync(int teamId)
    {
        var sprints = await _context.Sprints
            .Include(s => s.Tasks)
            .Where(s => s.TeamId == teamId)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();

        return sprints.Select(MapToSprintSummaryDto).ToList();
    }

    public async Task<SprintDto?> UpdateSprintAsync(int sprintId, UpdateSprintDto dto)
    {
        var sprint = await _context.Sprints.FindAsync(sprintId);
        if (sprint == null) return null;

        if (dto.Name != null) sprint.Name = dto.Name;
        if (dto.Goal != null) sprint.Goal = dto.Goal;
        if (dto.StartDate.HasValue) sprint.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) sprint.EndDate = dto.EndDate.Value;
        if (dto.Status.HasValue) sprint.Status = dto.Status.Value;

        await _context.SaveChangesAsync();

        return await GetSprintByIdAsync(sprintId);
    }

    public async Task<bool> DeleteSprintAsync(int sprintId)
    {
        var sprint = await _context.Sprints.FindAsync(sprintId);
        if (sprint == null) return false;

        _context.Sprints.Remove(sprint);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<SprintDto?> GetActiveSprintByTeamAsync(int teamId)
    {
        var sprint = await _context.Sprints
            .Include(s => s.Team)
            .Include(s => s.Tasks)
            .ThenInclude(t => t.Assignee)
            .Include(s => s.Tasks)
            .ThenInclude(t => t.Blockers)
            .Include(s => s.BurndownData)
            .Where(s => s.TeamId == teamId && s.Status == SprintStatus.Active)
            .FirstOrDefaultAsync();

        return sprint == null ? null : MapToSprintDto(sprint);
    }

    public async Task UpdateBurndownDataAsync(int sprintId)
    {
        var sprint = await _context.Sprints
            .Include(s => s.Tasks)
            .Include(s => s.BurndownData)
            .FirstOrDefaultAsync(s => s.Id == sprintId);

        if (sprint == null) return;

        var today = DateTime.UtcNow.Date;
        var existingEntry = sprint.BurndownData.FirstOrDefault(b => b.Date.Date == today);

        var totalPoints = sprint.Tasks.Sum(t => t.StoryPoints);
        var completedPoints = sprint.Tasks
            .Where(t => t.Status == TaskStatus.Done)
            .Sum(t => t.StoryPoints);
        var remainingPoints = totalPoints - completedPoints;

        var totalDays = (sprint.EndDate - sprint.StartDate).Days;
        var daysElapsed = (today - sprint.StartDate).Days;
        var idealRemaining = totalDays > 0 
            ? (int)(totalPoints * (1 - (double)daysElapsed / totalDays))
            : 0;

        if (existingEntry != null)
        {
            existingEntry.RemainingPoints = remainingPoints;
            existingEntry.CompletedPoints = completedPoints;
            existingEntry.IdealRemainingPoints = Math.Max(0, idealRemaining);
        }
        else
        {
            _context.SprintBurndowns.Add(new SprintBurndown
            {
                SprintId = sprintId,
                Date = today,
                RemainingPoints = remainingPoints,
                CompletedPoints = completedPoints,
                IdealRemainingPoints = Math.Max(0, idealRemaining)
            });
        }

        sprint.TotalStoryPoints = totalPoints;
        sprint.CompletedStoryPoints = completedPoints;

        await _context.SaveChangesAsync();
    }

    public async Task<List<BurndownDataDto>> GetBurndownDataAsync(int sprintId)
    {
        var data = await _context.SprintBurndowns
            .Where(b => b.SprintId == sprintId)
            .OrderBy(b => b.Date)
            .ToListAsync();

        return data.Select(b => new BurndownDataDto
        {
            Date = b.Date,
            RemainingPoints = b.RemainingPoints,
            IdealRemainingPoints = b.IdealRemainingPoints,
            CompletedPoints = b.CompletedPoints
        }).ToList();
    }

    private async Task InitializeBurndownDataAsync(Sprint sprint)
    {
        var totalDays = (sprint.EndDate - sprint.StartDate).Days;
        
        for (int i = 0; i <= totalDays; i++)
        {
            var date = sprint.StartDate.AddDays(i);
            _context.SprintBurndowns.Add(new SprintBurndown
            {
                SprintId = sprint.Id,
                Date = date,
                RemainingPoints = 0,
                CompletedPoints = 0,
                IdealRemainingPoints = 0
            });
        }

        await _context.SaveChangesAsync();
    }

    private SprintDto MapToSprintDto(Sprint sprint)
    {
        var totalTasks = sprint.Tasks.Count;
        var completedTasks = sprint.Tasks.Count(t => t.Status == TaskStatus.Done);
        var daysRemaining = Math.Max(0, (sprint.EndDate - DateTime.UtcNow).Days);

        return new SprintDto
        {
            Id = sprint.Id,
            Name = sprint.Name,
            Goal = sprint.Goal,
            TeamId = sprint.TeamId,
            TeamName = sprint.Team?.Name ?? "",
            StartDate = sprint.StartDate,
            EndDate = sprint.EndDate,
            Status = sprint.Status,
            TotalStoryPoints = sprint.TotalStoryPoints,
            CompletedStoryPoints = sprint.CompletedStoryPoints,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            ProgressPercentage = totalTasks > 0 ? Math.Round((double)completedTasks / totalTasks * 100, 1) : 0,
            DaysRemaining = daysRemaining,
            Tasks = sprint.Tasks.Select(t => new SprintTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                SprintId = t.SprintId,
                AssigneeId = t.AssigneeId,
                AssigneeName = t.Assignee?.FullName,
                AssigneeAvatar = t.Assignee?.AvatarUrl,
                Status = t.Status,
                Priority = t.Priority,
                StoryPoints = t.StoryPoints,
                OrderIndex = t.OrderIndex,
                IsBlocked = t.IsBlocked,
                BlockedReason = t.BlockedReason,
                CreatedAt = t.CreatedAt,
                StartedAt = t.StartedAt,
                CompletedAt = t.CompletedAt,
                BlockerCount = t.Blockers.Count(b => b.Status != BlockerStatus.Resolved)
            }).OrderBy(t => t.OrderIndex).ToList(),
            BurndownData = sprint.BurndownData.Select(b => new BurndownDataDto
            {
                Date = b.Date,
                RemainingPoints = b.RemainingPoints,
                IdealRemainingPoints = b.IdealRemainingPoints,
                CompletedPoints = b.CompletedPoints
            }).OrderBy(b => b.Date).ToList()
        };
    }

    private SprintSummaryDto MapToSprintSummaryDto(Sprint sprint)
    {
        var totalTasks = sprint.Tasks.Count;
        var completedTasks = sprint.Tasks.Count(t => t.Status == TaskStatus.Done);
        var daysRemaining = Math.Max(0, (sprint.EndDate - DateTime.UtcNow).Days);

        return new SprintSummaryDto
        {
            Id = sprint.Id,
            Name = sprint.Name,
            Goal = sprint.Goal,
            Status = sprint.Status,
            StartDate = sprint.StartDate,
            EndDate = sprint.EndDate,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            ProgressPercentage = totalTasks > 0 ? Math.Round((double)completedTasks / totalTasks * 100, 1) : 0,
            DaysRemaining = daysRemaining
        };
    }
}
