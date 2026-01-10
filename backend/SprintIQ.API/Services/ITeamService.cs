using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface ITeamService
{
    Task<TeamDto?> CreateTeamAsync(CreateTeamDto dto, int creatorUserId);
    Task<TeamDto?> GetTeamByIdAsync(int teamId);
    Task<List<TeamDto>> GetAllTeamsAsync();
    Task<List<TeamDto>> GetUserTeamsAsync(int userId);
    Task<TeamDto?> UpdateTeamAsync(int teamId, UpdateTeamDto dto);
    Task<bool> DeleteTeamAsync(int teamId);
    Task<TeamDto?> AddMemberAsync(int teamId, AddTeamMemberDto dto);
    Task<bool> RemoveMemberAsync(int teamId, int userId);
}
