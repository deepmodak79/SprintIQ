using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;

namespace SprintIQ.API.Services;

public class BlockerService : IBlockerService
{
    private readonly SprintIQDbContext _context;
    private readonly ILeaderboardService _leaderboardService;

    public BlockerService(SprintIQDbContext context, ILeaderboardService leaderboardService)
    {
        _context = context;
        _leaderboardService = leaderboardService;
    }

    public async Task<BlockerDto?> CreateBlockerAsync(int reportedByUserId, CreateBlockerDto dto)
    {
        var sprint = await _context.Sprints.FindAsync(dto.SprintId);
        if (sprint == null) return null;

        var blocker = new Blocker
        {
            Description = dto.Description,
            TaskId = dto.TaskId,
            SprintId = dto.SprintId,
            ReportedById = reportedByUserId,
            AssignedToId = dto.AssignedToId,
            Severity = dto.Severity,
            Status = BlockerStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        // Update task if linked
        if (dto.TaskId.HasValue)
        {
            var task = await _context.Tasks.FindAsync(dto.TaskId.Value);
            if (task != null)
            {
                task.IsBlocked = true;
                task.BlockedReason = dto.Description;
            }
        }

        _context.Blockers.Add(blocker);
        await _context.SaveChangesAsync();

        return await GetBlockerByIdAsync(blocker.Id);
    }

    public async Task<BlockerDto?> GetBlockerByIdAsync(int blockerId)
    {
        var blocker = await _context.Blockers
            .Include(b => b.Task)
            .Include(b => b.ReportedBy)
            .Include(b => b.AssignedTo)
            .FirstOrDefaultAsync(b => b.Id == blockerId);

        return blocker == null ? null : MapToBlockerDto(blocker);
    }

    public async Task<List<BlockerDto>> GetBlockersBySprintAsync(int sprintId)
    {
        var blockers = await _context.Blockers
            .Include(b => b.Task)
            .Include(b => b.ReportedBy)
            .Include(b => b.AssignedTo)
            .Where(b => b.SprintId == sprintId)
            .OrderByDescending(b => b.Severity)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync();

        return blockers.Select(MapToBlockerDto).ToList();
    }

    public async Task<List<BlockerDto>> GetOpenBlockersAsync(int? sprintId = null)
    {
        var query = _context.Blockers
            .Include(b => b.Task)
            .Include(b => b.ReportedBy)
            .Include(b => b.AssignedTo)
            .Where(b => b.Status != BlockerStatus.Resolved);

        if (sprintId.HasValue)
        {
            query = query.Where(b => b.SprintId == sprintId.Value);
        }

        var blockers = await query
            .OrderByDescending(b => b.Severity)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync();

        return blockers.Select(MapToBlockerDto).ToList();
    }

    public async Task<BlockerDto?> UpdateBlockerAsync(int blockerId, UpdateBlockerDto dto, int? resolvedByUserId = null)
    {
        var blocker = await _context.Blockers
            .Include(b => b.Task)
            .FirstOrDefaultAsync(b => b.Id == blockerId);

        if (blocker == null) return null;

        if (dto.Description != null) blocker.Description = dto.Description;
        if (dto.AssignedToId.HasValue) blocker.AssignedToId = dto.AssignedToId;
        if (dto.Status.HasValue) blocker.Status = dto.Status.Value;
        if (dto.Severity.HasValue) blocker.Severity = dto.Severity.Value;
        if (dto.Resolution != null) blocker.Resolution = dto.Resolution;

        if (dto.Status == BlockerStatus.Resolved)
        {
            blocker.ResolvedAt = DateTime.UtcNow;

            // Update linked task
            if (blocker.Task != null)
            {
                var hasOtherBlockers = await _context.Blockers
                    .AnyAsync(b => b.TaskId == blocker.TaskId && b.Id != blockerId && b.Status != BlockerStatus.Resolved);

                if (!hasOtherBlockers)
                {
                    blocker.Task.IsBlocked = false;
                    blocker.Task.BlockedReason = null;
                }
            }

            // Award points for resolving blocker
            if (resolvedByUserId.HasValue)
            {
                var points = blocker.Severity switch
                {
                    BlockerSeverity.Critical => 50,
                    BlockerSeverity.High => 35,
                    BlockerSeverity.Medium => 25,
                    _ => 15
                };

                await _leaderboardService.AddPointsAsync(resolvedByUserId.Value, points, "BlockerResolved", $"Resolved blocker: {blocker.Description}");
                await _leaderboardService.CheckAndAwardBadgesAsync(resolvedByUserId.Value);
            }
        }

        await _context.SaveChangesAsync();

        return await GetBlockerByIdAsync(blockerId);
    }

    public async Task<BlockerDto?> ResolveBlockerAsync(int blockerId, string resolution, int resolvedByUserId)
    {
        var dto = new UpdateBlockerDto
        {
            Status = BlockerStatus.Resolved,
            Resolution = resolution
        };

        return await UpdateBlockerAsync(blockerId, dto, resolvedByUserId);
    }

    public async Task<bool> DeleteBlockerAsync(int blockerId)
    {
        var blocker = await _context.Blockers.FindAsync(blockerId);
        if (blocker == null) return false;

        _context.Blockers.Remove(blocker);
        await _context.SaveChangesAsync();

        return true;
    }

    private static BlockerDto MapToBlockerDto(Blocker blocker)
    {
        return new BlockerDto
        {
            Id = blocker.Id,
            Description = blocker.Description,
            TaskId = blocker.TaskId,
            TaskTitle = blocker.Task?.Title,
            ReportedById = blocker.ReportedById,
            ReportedByName = blocker.ReportedBy?.FullName ?? "",
            AssignedToId = blocker.AssignedToId,
            AssignedToName = blocker.AssignedTo?.FullName,
            SprintId = blocker.SprintId,
            Status = blocker.Status,
            Severity = blocker.Severity,
            Resolution = blocker.Resolution,
            CreatedAt = blocker.CreatedAt,
            ResolvedAt = blocker.ResolvedAt,
            AiSuggestion = blocker.AiSuggestion,
            DaysOpen = blocker.ResolvedAt.HasValue
                ? (blocker.ResolvedAt.Value - blocker.CreatedAt).Days
                : (DateTime.UtcNow - blocker.CreatedAt).Days
        };
    }
}
