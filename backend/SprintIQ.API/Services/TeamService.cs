using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;

namespace SprintIQ.API.Services;

public class TeamService : ITeamService
{
    private readonly SprintIQDbContext _context;

    public TeamService(SprintIQDbContext context)
    {
        _context = context;
    }

    public async Task<TeamDto?> CreateTeamAsync(CreateTeamDto dto, int creatorUserId)
    {
        var team = new Team
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Add creator as team lead
        var membership = new TeamMember
        {
            TeamId = team.Id,
            UserId = creatorUserId,
            Role = "Lead",
            JoinedAt = DateTime.UtcNow
        };

        _context.TeamMembers.Add(membership);
        await _context.SaveChangesAsync();

        return await GetTeamByIdAsync(team.Id);
    }

    public async Task<TeamDto?> GetTeamByIdAsync(int teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .Include(t => t.Sprints)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        return team == null ? null : MapToTeamDto(team);
    }

    public async Task<List<TeamDto>> GetAllTeamsAsync()
    {
        var teams = await _context.Teams
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .Include(t => t.Sprints)
            .Where(t => t.IsActive)
            .ToListAsync();

        return teams.Select(MapToTeamDto).ToList();
    }

    public async Task<List<TeamDto>> GetUserTeamsAsync(int userId)
    {
        var teams = await _context.Teams
            .Include(t => t.Members)
            .ThenInclude(m => m.User)
            .Include(t => t.Sprints)
            .Where(t => t.IsActive && t.Members.Any(m => m.UserId == userId))
            .ToListAsync();

        return teams.Select(MapToTeamDto).ToList();
    }

    public async Task<TeamDto?> UpdateTeamAsync(int teamId, UpdateTeamDto dto)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team == null) return null;

        if (dto.Name != null) team.Name = dto.Name;
        if (dto.Description != null) team.Description = dto.Description;

        await _context.SaveChangesAsync();

        return await GetTeamByIdAsync(teamId);
    }

    public async Task<bool> DeleteTeamAsync(int teamId)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team == null) return false;

        team.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TeamDto?> AddMemberAsync(int teamId, AddTeamMemberDto dto)
    {
        var team = await _context.Teams.FindAsync(teamId);
        var user = await _context.Users.FindAsync(dto.UserId);

        if (team == null || user == null) return null;

        var existingMembership = await _context.TeamMembers
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == dto.UserId);

        if (existingMembership != null) return await GetTeamByIdAsync(teamId);

        var membership = new TeamMember
        {
            TeamId = teamId,
            UserId = dto.UserId,
            Role = dto.Role,
            JoinedAt = DateTime.UtcNow
        };

        _context.TeamMembers.Add(membership);
        await _context.SaveChangesAsync();

        return await GetTeamByIdAsync(teamId);
    }

    public async Task<bool> RemoveMemberAsync(int teamId, int userId)
    {
        var membership = await _context.TeamMembers
            .FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);

        if (membership == null) return false;

        _context.TeamMembers.Remove(membership);
        await _context.SaveChangesAsync();

        return true;
    }

    private static TeamDto MapToTeamDto(Team team)
    {
        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            CreatedAt = team.CreatedAt,
            MemberCount = team.Members.Count,
            ActiveSprintCount = team.Sprints.Count(s => s.Status == SprintStatus.Active),
            Members = team.Members.Select(m => new TeamMemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User?.FullName ?? "",
                UserEmail = m.User?.Email,
                AvatarUrl = m.User?.AvatarUrl,
                Role = m.Role,
                JoinedAt = m.JoinedAt,
                TotalPoints = m.User?.TotalPoints ?? 0
            }).ToList()
        };
    }
}
