using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;

namespace SprintIQ.API.Services;

public class StandupService : IStandupService
{
    private readonly SprintIQDbContext _context;
    private readonly ILeaderboardService _leaderboardService;

    public StandupService(SprintIQDbContext context, ILeaderboardService leaderboardService)
    {
        _context = context;
        _leaderboardService = leaderboardService;
    }

    public async Task<StandupDto?> CreateOrUpdateStandupAsync(int userId, CreateStandupDto dto)
    {
        var sprint = await _context.Sprints.FindAsync(dto.SprintId);
        if (sprint == null) return null;

        var today = DateTime.UtcNow.Date;
        var existingStandup = await _context.DailyStandups
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SprintId == dto.SprintId && s.Date == today);

        var isNewStandup = existingStandup == null;

        if (existingStandup != null)
        {
            existingStandup.Yesterday = dto.Yesterday;
            existingStandup.Today = dto.Today;
            existingStandup.Blockers = dto.Blockers;
            existingStandup.Mood = dto.Mood;
            existingStandup.Confidence = dto.Confidence;
            existingStandup.SubmittedAt = DateTime.UtcNow;
        }
        else
        {
            existingStandup = new DailyStandup
            {
                UserId = userId,
                SprintId = dto.SprintId,
                Date = today,
                Yesterday = dto.Yesterday,
                Today = dto.Today,
                Blockers = dto.Blockers,
                Mood = dto.Mood,
                Confidence = dto.Confidence,
                SubmittedAt = DateTime.UtcNow,
                PointsEarned = 10
            };

            _context.DailyStandups.Add(existingStandup);
        }

        await _context.SaveChangesAsync();

        // Award points for new standup
        if (isNewStandup)
        {
            await _leaderboardService.AddPointsAsync(userId, 10, "StandupSubmitted", "Daily standup submitted");
            await UpdateUserStreakAsync(userId);
            await _leaderboardService.CheckAndAwardBadgesAsync(userId);
        }

        return await GetStandupByIdAsync(existingStandup.Id);
    }

    public async Task<StandupDto?> GetStandupByIdAsync(int standupId)
    {
        var standup = await _context.DailyStandups
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == standupId);

        return standup == null ? null : MapToStandupDto(standup);
    }

    public async Task<StandupDto?> GetTodayStandupAsync(int userId, int sprintId)
    {
        var today = DateTime.UtcNow.Date;
        var standup = await _context.DailyStandups
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SprintId == sprintId && s.Date == today);

        return standup == null ? null : MapToStandupDto(standup);
    }

    public async Task<List<StandupDto>> GetStandupsBySprintAsync(int sprintId, DateTime? date = null)
    {
        var query = _context.DailyStandups
            .Include(s => s.User)
            .Where(s => s.SprintId == sprintId);

        if (date.HasValue)
        {
            query = query.Where(s => s.Date == date.Value.Date);
        }

        var standups = await query
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();

        return standups.Select(MapToStandupDto).ToList();
    }

    public async Task<TeamStandupSummaryDto> GetTeamStandupSummaryAsync(int sprintId, DateTime date)
    {
        var sprint = await _context.Sprints
            .Include(s => s.Team)
            .ThenInclude(t => t.Members)
            .FirstOrDefaultAsync(s => s.Id == sprintId);

        if (sprint == null)
        {
            return new TeamStandupSummaryDto { Date = date };
        }

        var standups = await GetStandupsBySprintAsync(sprintId, date);
        var totalMembers = sprint.Team.Members.Count;

        var blockersList = standups
            .Where(s => !string.IsNullOrEmpty(s.Blockers))
            .Select(s => s.Blockers!)
            .ToList();

        return new TeamStandupSummaryDto
        {
            Date = date,
            TotalMembers = totalMembers,
            SubmittedCount = standups.Count,
            AverageMood = standups.Where(s => s.Mood.HasValue).Average(s => s.Mood!.Value),
            AverageConfidence = standups.Where(s => s.Confidence.HasValue).Average(s => s.Confidence!.Value),
            Standups = standups,
            CommonBlockers = blockersList
        };
    }

    public async Task<List<StandupDto>> GetUserStandupsAsync(int userId, int? limit = null)
    {
        var query = _context.DailyStandups
            .Include(s => s.User)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Date);

        if (limit.HasValue)
        {
            query = (IOrderedQueryable<DailyStandup>)query.Take(limit.Value);
        }

        var standups = await query.ToListAsync();
        return standups.Select(MapToStandupDto).ToList();
    }

    private async Task UpdateUserStreakAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var hadStandupYesterday = await _context.DailyStandups
            .AnyAsync(s => s.UserId == userId && s.Date == yesterday);

        if (hadStandupYesterday)
        {
            user.CurrentStreak++;
        }
        else
        {
            user.CurrentStreak = 1;
        }

        if (user.CurrentStreak > user.LongestStreak)
        {
            user.LongestStreak = user.CurrentStreak;
        }

        await _context.SaveChangesAsync();
    }

    private static StandupDto MapToStandupDto(DailyStandup standup)
    {
        return new StandupDto
        {
            Id = standup.Id,
            UserId = standup.UserId,
            UserName = standup.User?.FullName ?? "",
            UserAvatar = standup.User?.AvatarUrl,
            SprintId = standup.SprintId,
            Date = standup.Date,
            Yesterday = standup.Yesterday,
            Today = standup.Today,
            Blockers = standup.Blockers,
            Mood = standup.Mood,
            Confidence = standup.Confidence,
            SubmittedAt = standup.SubmittedAt,
            AiSummary = standup.AiSummary,
            PointsEarned = standup.PointsEarned
        };
    }
}
