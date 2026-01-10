using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

public interface IBlockerService
{
    Task<BlockerDto?> CreateBlockerAsync(int reportedByUserId, CreateBlockerDto dto);
    Task<BlockerDto?> GetBlockerByIdAsync(int blockerId);
    Task<List<BlockerDto>> GetBlockersBySprintAsync(int sprintId);
    Task<List<BlockerDto>> GetOpenBlockersAsync(int? sprintId = null);
    Task<BlockerDto?> UpdateBlockerAsync(int blockerId, UpdateBlockerDto dto, int? resolvedByUserId = null);
    Task<BlockerDto?> ResolveBlockerAsync(int blockerId, string resolution, int resolvedByUserId);
    Task<bool> DeleteBlockerAsync(int blockerId);
}
